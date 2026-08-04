using System;

namespace ConfigSync.Domain;

public sealed class DeviceCredential
{
    public static DeviceCredential Password(string plaintext) => new(DeviceAuthenticationType.Password, plaintext);

    public static DeviceCredential PrivateKey(string plaintext) => new(DeviceAuthenticationType.PrivateKey, plaintext);

    public DeviceAuthenticationType AuthenticationType { get; }

    public string GetPlaintext() => _plaintext;

    public override string ToString() => "***";

    private readonly string _plaintext;

    private DeviceCredential(DeviceAuthenticationType authenticationType, string plaintext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintext);

        AuthenticationType = authenticationType;
        _plaintext = plaintext;
    }
}
