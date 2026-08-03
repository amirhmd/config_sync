using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ConfigSync.Infrastructure.Postgres;

internal static class PostgresDependencyInjection
{
    internal static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<PostgresOptions>()
            .Bind(configuration.GetSection(PostgresOptions.SectionName))
            .Validate(
                options => options.MaximumPoolSize > 0,
                "Postgres:MaximumPoolSize must be explicitly configured and greater than zero.")
            .Validate(
                options => options.MinimumPoolSize >= 0,
                "Postgres:MinimumPoolSize cannot be negative.")
            .Validate(
                options => options.MinimumPoolSize <= options.MaximumPoolSize,
                "Postgres:MinimumPoolSize cannot exceed MaximumPoolSize.")
            .Validate(
                options => options.ConnectionTimeoutSeconds is > 0 and <= 300,
                "Postgres:ConnectionTimeoutSeconds must be between 1 and 300.")
            .Validate(
                options => options.CommandTimeoutSeconds is > 0 and <= 3600,
                "Postgres:CommandTimeoutSeconds must be between 1 and 3600.")
            .ValidateOnStart();

        var connectionString = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'Postgres' is missing.");

        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PostgresOptions>>().Value;
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Pooling = true,
                MinPoolSize = options.MinimumPoolSize,
                MaxPoolSize = options.MaximumPoolSize,
                Timeout = options.ConnectionTimeoutSeconds,
                CommandTimeout = options.CommandTimeoutSeconds
            };

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);

            dataSourceBuilder.UseLoggerFactory(loggerFactory);
            dataSourceBuilder.Name = "ConfigSync.Postgres";

            return dataSourceBuilder.Build();
        });

        services
            .AddHealthChecks()
            .AddCheck<PostgresHealthCheck>(
                name: "postgres",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["readiness"],
                timeout: TimeSpan.FromSeconds(2));

        return services;
    }
}
