using System;
using System.Security.Cryptography;
using System.Text;
using ConfigSync.Adapters.Out.Persistence.Postgres.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Postgres.Credentials;

public class DeviceCredentialEncryptionTests
{
    private static IDeviceCredentialEncryption BuildEncryption()
    {
        var services = new ServiceCollection();
        services.AddDataProtection().SetApplicationName("ConfigSync");
        services.AddSingleton<IDeviceCredentialEncryption, DeviceCredentialEncryption>();
        return services.BuildServiceProvider().GetRequiredService<IDeviceCredentialEncryption>();
    }

    // --- EncryptPassword ---

    [Fact]
    public void EncryptPassword_ReturnsNonEmptyBytes()
    {
        // given
        var encryption = BuildEncryption();

        // when
        var result = encryption.EncryptPassword("secret123");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void EncryptPassword_CiphertextDoesNotContainPlaintext()
    {
        // given
        var encryption = BuildEncryption();
        const string password = "secret123";

        // when
        var result = encryption.EncryptPassword(password);

        // then
        Assert.NotEqual(Encoding.UTF8.GetBytes(password), result);
        Assert.DoesNotContain(password, Convert.ToBase64String(result), StringComparison.Ordinal);
    }

    [Fact]
    public void EncryptPassword_SameInputProducesDifferentCiphertext()
    {
        // given
        var encryption = BuildEncryption();

        // when
        var first = encryption.EncryptPassword("secret123");
        var second = encryption.EncryptPassword("secret123");

        // then
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void EncryptPassword_NullPassword_Throws()
    {
        // given
        var encryption = BuildEncryption();

        // when / then
        Assert.ThrowsAny<ArgumentException>(() => encryption.EncryptPassword(null!));
    }

    [Fact]
    public void EncryptPassword_WhitespacePassword_Throws()
    {
        // given
        var encryption = BuildEncryption();

        // when / then
        Assert.Throws<ArgumentException>(() => encryption.EncryptPassword("   "));
    }

    // --- EncryptPrivateKey ---

    [Fact]
    public void EncryptPrivateKey_ReturnsNonEmptyBytes()
    {
        // given
        var encryption = BuildEncryption();

        // when
        var result = encryption.EncryptPrivateKey("-----BEGIN OPENSSH PRIVATE KEY-----");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

   
    [Fact]
    public void EncryptPrivateKey_NullPrivateKey_Throws()
    {
        // given
        var encryption = BuildEncryption();

        // when / then
        Assert.ThrowsAny<ArgumentException>(() => encryption.EncryptPrivateKey(null!));
    }

    // --- DecryptPassword round-trip ---

    [Fact]
    public void DecryptPassword_RoundTrip_ReturnsOriginalValue()
    {
        // given
        var encryption = BuildEncryption();
        const string original = "my-secret-password";
        var encrypted = encryption.EncryptPassword(original);

        // when
        var decrypted = encryption.DecryptPassword(encrypted);

        // then
        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void DecryptPassword_NullBytes_Throws()
    {
        // given
        var encryption = BuildEncryption();

        // when / then
        Assert.Throws<ArgumentNullException>(() => encryption.DecryptPassword(null!));
    }

    [Fact]
    public void DecryptPassword_CorruptedBytes_Throws()
    {
        // given
        var encryption = BuildEncryption();
        var corrupted = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        // when / then
        Assert.ThrowsAny<CryptographicException>(() => encryption.DecryptPassword(corrupted));
    }

    // --- DecryptPrivateKey round-trip ---

    [Fact]
    public void DecryptPrivateKey_RoundTrip_ReturnsOriginalValue()
    {
        // given
        var encryption = BuildEncryption();
        const string original = "-----BEGIN OPENSSH PRIVATE KEY-----";
        var encrypted = encryption.EncryptPrivateKey(original);

        // when
        var decrypted = encryption.DecryptPrivateKey(encrypted);

        // then
        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void DecryptPrivateKey_NullBytes_Throws()
    {
        // given
        var encryption = BuildEncryption();

        // when / then
        Assert.Throws<ArgumentNullException>(() => encryption.DecryptPrivateKey(null!));
    }

    // --- Cross-credential isolation ---

    [Fact]
    public void DecryptPassword_WithPrivateKeyBytes_Throws()
    {
        // given
        var encryption = BuildEncryption();
        var encryptedPrivateKey = encryption.EncryptPrivateKey("my-key");

        // when / then
        // Separate protectors per credential type mean that bytes
        // encrypted under the private-key purpose cannot be decrypted
        // under the password purpose — the cryptographic layer enforces
        // type isolation, not just the database column.
        Assert.Throws<CryptographicException>(() =>
            encryption.DecryptPassword(encryptedPrivateKey));
    }

    [Fact]
    public void DecryptPrivateKey_WithPasswordBytes_Throws()
    {
        // given
        var encryption = BuildEncryption();
        var encryptedPassword = encryption.EncryptPassword("my-password");

        // when / then
        Assert.Throws<CryptographicException>(() =>
            encryption.DecryptPrivateKey(encryptedPassword));
    }
}