namespace ConfigSync.Adapters.In.Rest.Models.Requests;

public sealed record CreateDeviceRequest(
    string Name,
    string Host,
    int Port,
    string Username,
    string? Password,
    string? PrivateKey);
