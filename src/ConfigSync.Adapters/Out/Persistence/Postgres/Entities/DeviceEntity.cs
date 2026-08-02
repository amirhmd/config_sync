namespace ConfigSync.Adapters.Out.Persistence.Postgres.Entities;

public sealed class DeviceEntity
{
    public required string Id { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public string? Password { get; init; }
    public string? PrivateKey { get; init; }
}