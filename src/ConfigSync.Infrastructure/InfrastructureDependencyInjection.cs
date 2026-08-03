using System;
using System.IO;
using ConfigSync.Infrastructure.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var keyPath = configuration["DataProtection:KeyPath"] ?? Path.Combine(AppContext.BaseDirectory, ".keys");

        services
            .AddDataProtection()
            .SetApplicationName("ConfigSync")
            .PersistKeysToFileSystem(new DirectoryInfo(keyPath));

        services.AddSingleton<IDeviceCredentialEncryption, DeviceCredentialEncryption>();

        return services;
    }
}