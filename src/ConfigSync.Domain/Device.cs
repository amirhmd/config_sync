namespace ConfigSync.Domain;

public sealed record Device(
    string Id,
    string Host,
    int Port,
    string Username,
    string? Password,
    string? PrivateKey);