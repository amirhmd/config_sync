using ConfigSync.Adapters.Out.Persistence.Postgres.Entities;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.Out.Persistence.Postgres.Mappers;

public interface IDeviceEntityMapper
{
    /// <summary>
    /// Builds a domain <see cref="Device"/> from a stored row. The credential keeps its stored
    /// ciphertext; nothing is decrypted here. Throws when the row does not carry exactly one
    /// credential, which the database CHECK constraint is expected to prevent.
    /// </summary>
    Device ToDomain(DeviceEntity entity);
}
