using System;
using Xunit;

namespace ConfigSync.Domain.Tests;

public class DeviceTests
{
    private static Device NewDevice(DeviceCredential credential) =>
        new("router_01", "localhost", 2201, "device", credential);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        // given
        var credential = DeviceCredential.Password("s3cr3t");

        // when
        var device = NewDevice(credential);

        // then
        Assert.Equal("router_01", device.Name);
        Assert.Equal("localhost", device.Host);
        Assert.Equal(2201, device.Port);
        Assert.Equal("device", device.Username);
        Assert.Same(credential, device.Credential);
    }

    [Fact]
    public void AuthenticationType_ForPasswordCredential_IsPassword()
    {
        // given
        var device = NewDevice(DeviceCredential.Password("pw"));

        // when
        var authenticationType = device.AuthenticationType;

        // then
        Assert.Equal(DeviceAuthenticationType.Password, authenticationType);
    }

    [Fact]
    public void AuthenticationType_ForPrivateKeyCredential_IsPrivateKey()
    {
        // given
        var device = NewDevice(DeviceCredential.PrivateKey("-----BEGIN KEY-----"));

        // when
        var authenticationType = device.AuthenticationType;

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, authenticationType);
    }

    [Fact]
    public void GetPlaintext_RoundTripsTheSecret()
    {
        // given
        const string secret = "super-secret-value";
        var credential = DeviceCredential.Password(secret);

        // when
        var plaintext = credential.GetPlaintext();

        // then
        Assert.Equal(secret, plaintext);
    }

    [Fact]
    public void ToString_DoesNotContainSecret()
    {
        // given
        const string secret = "super-secret-value";
        var credential = DeviceCredential.PrivateKey(secret);

        // when
        var text = credential.ToString();

        // then
        Assert.DoesNotContain(secret, text);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Factory_RejectsBlankPlaintext(string blank)
    {
        // given / when / then
        Assert.Throws<ArgumentException>(() => DeviceCredential.Password(blank));
    }

    [Fact]
    public void Factory_RejectsNullPlaintext()
    {
        // given / when / then
        Assert.Throws<ArgumentNullException>(() => DeviceCredential.PrivateKey(null!));
    }
}
