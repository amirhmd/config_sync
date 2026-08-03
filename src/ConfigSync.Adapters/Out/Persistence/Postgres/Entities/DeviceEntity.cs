using System;

namespace ConfigSync.Adapters.Out.Persistence.Postgres.Entities;

public sealed class DeviceEntity
{
    public required string Id { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string AuthenticationType { get; init; }
    public byte[]? PasswordEncrypted { get; init; }
    public byte[]? PrivateKeyEncrypted { get; init; }
    public required long Version { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
