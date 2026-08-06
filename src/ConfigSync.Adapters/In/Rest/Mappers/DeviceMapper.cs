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
        return new Device(
            request.Name,
            request.Host,
            request.Port,
            request.Username,
            CreateCredential(request));
    }

    public DeviceDetails ToDetails(Device device)
    {
        return new DeviceDetails(
            device.Name,
            device.Host,
            device.Port,
            device.Username,
            ToWireAuthenticationType(device.AuthenticationType));
    }

    public DevicePageDetails ToPageDetails(Page<Device> page)
    {
        var items = page.Items.Select(ToDetails).ToList();

        return new DevicePageDetails(items, page.NextCursor);
    }

    public CreateDeviceOutcome ToCreateOutcome(CreateOutcome outcome)
    {
        if (outcome == CreateOutcome.Created)
        {
            return CreateDeviceOutcome.Created;
        }

        if (outcome == CreateOutcome.AlreadyExists)
        {
            return CreateDeviceOutcome.AlreadyExists;
        }

        throw new ArgumentOutOfRangeException(nameof(outcome), outcome.Value, "Unsupported create outcome.");
    }

    public DeleteDeviceOutcome ToDeleteOutcome(DeleteOutcome outcome)
    {
        if (outcome == DeleteOutcome.Deleted)
        {
            return DeleteDeviceOutcome.Deleted;
        }

        if (outcome == DeleteOutcome.NotFound)
        {
            return DeleteDeviceOutcome.NotFound;
        }

        throw new ArgumentOutOfRangeException(nameof(outcome), outcome.Value, "Unsupported delete outcome.");
    }

    private DeviceCredential CreateCredential(CreateDeviceRequest request)
    {
        if (request.Password is not null)
        {
            return DeviceCredential.Password(encryption.EncryptPassword(request.Password));
        }

        if (request.PrivateKey is not null)
        {
            return DeviceCredential.PrivateKey(encryption.EncryptPrivateKey(request.PrivateKey));
        }

        throw new InvalidOperationException("Exactly one credential must be provided.");
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
