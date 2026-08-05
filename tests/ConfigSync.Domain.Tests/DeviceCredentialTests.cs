using System;
using Xunit;

namespace ConfigSync.Domain.Tests;

public class DeviceCredentialTests
{
    [Fact]
    public void Password_CreatesPasswordCredential()
    {
        // given
        var ciphertext = new byte[] { 0x0A, 0x0B, 0x0C };

        // when
        var credential = DeviceCredential.Password(ciphertext);

        // then
        Assert.Equal(DeviceAuthenticationType.Password, credential.AuthenticationType);
        Assert.Equal(ciphertext, credential.GetCiphertext());
    }

    [Fact]
    public void PrivateKey_CreatesPrivateKeyCredential()
    {
        // given
        var ciphertext = new byte[] { 0x0A, 0x0B, 0x0C };

        // when
        var credential = DeviceCredential.PrivateKey(ciphertext);

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, credential.AuthenticationType);
        Assert.Equal(ciphertext, credential.GetCiphertext());
    }

    [Fact]
    public void ToString_HidesCiphertext()
    {
        // given
        var credential = DeviceCredential.Password([0x0A, 0x0B, 0x0C]);

        // when
        var result = credential.ToString();

        // then
        Assert.Equal("***", result);
    }

    [Fact]
    public void Password_ThrowsWhenCiphertextIsNull()
    {
        // given / when / then
        Assert.Throws<ArgumentNullException>(
            () => DeviceCredential.Password(null!));
    }

    [Fact]
    public void Password_ThrowsWhenCiphertextIsEmpty()
    {
        // given / when / then
        Assert.Throws<ArgumentException>(
            () => DeviceCredential.Password([]));
    }

    [Fact]
    public void PrivateKey_ThrowsWhenCiphertextIsNull()
    {
        // given / when / then
        Assert.Throws<ArgumentNullException>(
            () => DeviceCredential.PrivateKey(null!));
    }

    [Fact]
    public void PrivateKey_ThrowsWhenCiphertextIsEmpty()
    {
        // given / when / then
        Assert.Throws<ArgumentException>(
            () => DeviceCredential.PrivateKey([]));
    }
}
