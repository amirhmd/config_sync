using System;
using ConfigSync.Adapters.Out.Persistence.Postgres.Entities;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.Out.Persistence.Postgres.Mappers;

public sealed class DeviceEntityMapper : IDeviceEntityMapper
{
    public Device ToDomain(DeviceEntity entity)
    {
        return new Device(
            entity.Name,
            entity.Host,
            entity.Port,
            entity.Username,
            ToCredential(entity));
    }

    // The credential type comes from which column is populated, not from authentication_type.
    // That column exists so the database CHECK can enforce the XOR
    private static DeviceCredential ToCredential(DeviceEntity entity)
    {
        if (entity.PasswordEncrypted is not null && entity.PrivateKeyEncrypted is null)
        {
            return DeviceCredential.Password(entity.PasswordEncrypted);
        }

        if (entity.PasswordEncrypted is null && entity.PrivateKeyEncrypted is not null)
        {
            return DeviceCredential.PrivateKey(entity.PrivateKeyEncrypted);
        }

        throw new InvalidOperationException($"Device row '{entity.Name}' contains an invalid credential state.");
    }
}
