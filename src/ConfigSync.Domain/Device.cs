namespace ConfigSync.Domain;

public sealed class Device(
    string name,
    string host,
    int port,
    string username,
    DeviceCredential credential)
{
    public string Name { get; } = name;
    public string Host { get; } = host;
    public int Port { get; } = port;
    public string Username { get; } = username;
    public DeviceCredential Credential { get; } = credential;

    public DeviceAuthenticationType AuthenticationType => Credential.AuthenticationType;
}