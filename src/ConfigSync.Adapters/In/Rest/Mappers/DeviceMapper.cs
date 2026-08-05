using System;
using System.Linq;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Domain;
using ConfigSync.Infrastructure.Credentials;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public sealed class DeviceMapper(IDeviceCredentialEncryption encryption) : IDeviceMapper
{
    public Device ToDevice(CreateDeviceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Device name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Host))
        {
            throw new InvalidOperationException("Device host is required.");
        }

        if (request.Port is null)
        {
            throw new InvalidOperationException("Device port is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new InvalidOperationException("Device username is required.");
        }

        return new Device(
            request.Name,
            request.Host,
            request.Port.Value,
            request.Username,
            CreateCredential(request));
    }

    public DeviceDetails ToDetails(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);

        return new DeviceDetails(
            device.Name,
            device.Host,
            device.Port,
            device.Username,
            ToWireAuthenticationType(device.AuthenticationType));
    }

    public DevicePageDetails ToPageDetails(Page<Device> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        var items = page.Items.Select(ToDetails).ToList();

        return new DevicePageDetails(items, page.NextCursor);
    }

    private DeviceCredential CreateCredential(CreateDeviceRequest request)
    {
        var hasPassword = !string.IsNullOrWhiteSpace(request.Password);
        var hasPrivateKey = !string.IsNullOrWhiteSpace(request.PrivateKey);

        if (hasPassword == hasPrivateKey)
        {
            throw new InvalidOperationException("Exactly one credential must be provided.");
        }

        if (hasPassword)
        {
            return DeviceCredential.Password(encryption.EncryptPassword(request.Password!));
        }

        return DeviceCredential.PrivateKey(encryption.EncryptPrivateKey(request.PrivateKey!));
    }

    private static string ToWireAuthenticationType(DeviceAuthenticationType authenticationType)
    {
        return authenticationType switch
        {
            DeviceAuthenticationType.Password => "password",
            DeviceAuthenticationType.PrivateKey => "private_key",
            _ => throw new ArgumentOutOfRangeException(
                nameof(authenticationType),
                authenticationType,
                "Unsupported authentication type.")
        };
    }
}
