using System;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using ConfigSync.Adapters.Out.Persistence.Postgres.Entities;
using ConfigSync.Application.Ports.In;
using ConfigSync.Infrastructure.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

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

    private static RequestValidation BuildValidation() =>
        new(
            new NameValidation(),
            new HostValidation(),
            new PortValidation(),
            new UsernameValidation(),
            new CredentialValidation(),
            new LimitValidation(),
            NullLogger<RequestValidation>.Instance);

    internal static DeviceEndpoint BuildEndpoint(IDeviceUseCase useCase) =>
        new(useCase, BuildMapper(BuildEncryption()), BuildValidation());

    internal static CreateDeviceRequest PasswordRequest(
        string name = "router_01",
        string password = "secret123") =>
        new(name, "localhost", 2201, "device", password, null);

    internal static CreateDeviceRequest PrivateKeyRequest(
        string name = "router_01",
        string privateKey = "-----BEGIN OPENSSH PRIVATE KEY-----") =>
        new(name, "localhost", 2201, "device", null, privateKey);

    internal static DeviceEntity DeviceRow(
        string name = "router_01",
        byte[]? passwordEncrypted = null,
        byte[]? privateKeyEncrypted = null) =>
        new()
        {
            Id = 1,
            Name = name,
            Host = "localhost",
            Port = 2201,
            Username = "device",
            PasswordEncrypted = passwordEncrypted,
            PrivateKeyEncrypted = privateKeyEncrypted,
            CreatedAt = DateTimeOffset.UtcNow
        };
}
