using System;

namespace ConfigSync.Domain;

public sealed class DeviceCredential
{
    public static DeviceCredential Password(byte[] ciphertext) =>
        new(DeviceAuthenticationType.Password, ciphertext);

    public static DeviceCredential PrivateKey(byte[] ciphertext) =>
        new(DeviceAuthenticationType.PrivateKey, ciphertext);

    public DeviceAuthenticationType AuthenticationType { get; }

    public byte[] GetCiphertext() => _ciphertext;

    public override string ToString() => "***";

    private readonly byte[] _ciphertext;

    private DeviceCredential(DeviceAuthenticationType authenticationType, byte[] ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);

        if (ciphertext.Length == 0)
        {
            throw new ArgumentException("Ciphertext cannot be empty.", nameof(ciphertext));
        }

        AuthenticationType = authenticationType;
        _ciphertext = ciphertext;
    }
}
