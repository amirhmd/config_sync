using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace ConfigSync.Infrastructure.Credentials;

public sealed class DeviceCredentialEncryption : IDeviceCredentialEncryption
{
    private const string PasswordPurpose = "ConfigSync.DeviceCredentials.Password.v1";
    private const string PrivateKeyPurpose = "ConfigSync.DeviceCredentials.PrivateKey.v1";

    private readonly IDataProtector _passwordProtector;
    private readonly IDataProtector _privateKeyProtector;

    public DeviceCredentialEncryption(IDataProtectionProvider dataProtectionProvider)
    {
        ArgumentNullException.ThrowIfNull(dataProtectionProvider);
        _passwordProtector = dataProtectionProvider.CreateProtector(PasswordPurpose);
        _privateKeyProtector = dataProtectionProvider.CreateProtector(PrivateKeyPurpose);
    }

    public byte[] EncryptPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return Encrypt(password, _passwordProtector);
    }

    public byte[] EncryptPrivateKey(string privateKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(privateKey);
        return Encrypt(privateKey, _privateKeyProtector);
    }

    public string DecryptPassword(byte[] encryptedPassword)
    {
        ArgumentNullException.ThrowIfNull(encryptedPassword);
        return Decrypt(encryptedPassword, _passwordProtector);
    }

    public string DecryptPrivateKey(byte[] encryptedPrivateKey)
    {
        ArgumentNullException.ThrowIfNull(encryptedPrivateKey);
        return Decrypt(encryptedPrivateKey, _privateKeyProtector);
    }

    private static byte[] Encrypt(string value, IDataProtector protector)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(value);
        try { return protector.Protect(plaintextBytes); }
        finally { CryptographicOperations.ZeroMemory(plaintextBytes); }
    }

    private static string Decrypt(byte[] encryptedValue, IDataProtector protector)
    {
        var plaintextBytes = protector.Unprotect(encryptedValue);
        try { return Encoding.UTF8.GetString(plaintextBytes); }
        finally { CryptographicOperations.ZeroMemory(plaintextBytes); }
    }
}