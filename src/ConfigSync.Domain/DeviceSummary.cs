namespace ConfigSync.Domain;

public sealed record DeviceSummary(
    string Name,
    string Host,
    int Port,
    string Username,
    DeviceAuthenticationType AuthenticationType);
