using System;
using ConfigSync.Adapters.Out.Persistence.Postgres.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal static class PostgresDependencyInjection
{
    internal static IServiceCollection AddPostgresAdapter(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<PostgresOptions>()
            .Bind(configuration.GetSection(PostgresOptions.SectionName))
            .Validate(options => options.MaximumPoolSize is > 0,
                "Postgres:MaximumPoolSize must be explicitly configured and greater than zero.")
            .Validate(options => options.MinimumPoolSize >= 0, "Postgres:MinimumPoolSize cannot be negative.")
            .Validate(options => options.MinimumPoolSize <= (options.MaximumPoolSize ?? 0),
                "Postgres:MinimumPoolSize cannot exceed MaximumPoolSize.")
            .ValidateOnStart();

        var connectionString = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Postgres' is missing.");
        }

        services.AddDataProtection().SetApplicationName("ConfigSync");

        services.AddSingleton<IDeviceCredentialEncryption, DeviceCredentialEncryption>();
        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PostgresOptions>>().Value;
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Pooling = true,
                MinPoolSize = options.MinimumPoolSize,
                MaxPoolSize = options.MaximumPoolSize!.Value,
                Timeout = options.ConnectionTimeoutSeconds,
                CommandTimeout = options.CommandTimeoutSeconds
            };

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);
            dataSourceBuilder.UseLoggerFactory(loggerFactory);
            dataSourceBuilder.Name = "ConfigSync.Postgres";
            return dataSourceBuilder.Build();
        });

        return services;
    }
}