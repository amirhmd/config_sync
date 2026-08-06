using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Infrastructure.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters.Tests;

internal static class AdaptersTestConfiguration
{
    internal static IDeviceCredentialEncryption BuildEncryption()
    {
        var services = new ServiceCollection();
        services.AddDataProtection().SetApplicationName("ConfigSync");
        services.AddSingleton<IDeviceCredentialEncryption, DeviceCredentialEncryption>();

        return services.BuildServiceProvider()
            .GetRequiredService<IDeviceCredentialEncryption>();
    }

    internal static DeviceMapper BuildMapper(IDeviceCredentialEncryption encryption) =>
        new(encryption);

    internal static CreateDeviceRequest PasswordRequest(
        string name = "router_01",
        string password = "secret123") =>
        new(name, "localhost", 2201, "device", password, null);

    internal static CreateDeviceRequest PrivateKeyRequest(
        string name = "router_01",
        string privateKey = "-----BEGIN OPENSSH PRIVATE KEY-----") =>
        new(name, "localhost", 2201, "device", null, privateKey);
}
