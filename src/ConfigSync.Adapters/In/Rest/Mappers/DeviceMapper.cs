using System;
using System.Linq;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public sealed class DeviceMapper : IDeviceMapper
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
            DeviceName.Normalize(request.Name),
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

    public DevicePageDetails ToPageDetails(Page<DeviceSummary> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        var items = page.Items
            .Select(summary => new DeviceDetails(
                summary.Name,
                summary.Host,
                summary.Port,
                summary.Username,
                ToWireAuthenticationType(summary.AuthenticationType)))
            .ToList();

        return new DevicePageDetails(items, page.NextCursor);
    }

    private static DeviceCredential CreateCredential(CreateDeviceRequest request)
    {
        var hasPassword = !string.IsNullOrWhiteSpace(request.Password);
        var hasPrivateKey = !string.IsNullOrWhiteSpace(request.PrivateKey);

        if (hasPassword == hasPrivateKey)
        {
            throw new InvalidOperationException("Exactly one credential must be provided.");
        }

        if (hasPassword)
        {
            return DeviceCredential.Password(request.Password!);
        }

        return DeviceCredential.PrivateKey(request.PrivateKey!);
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