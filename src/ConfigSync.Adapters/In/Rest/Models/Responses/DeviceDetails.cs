namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record DeviceDetails(
    string Name,
    string Host,
    int Port,
    string Username,
    string AuthenticationType);
