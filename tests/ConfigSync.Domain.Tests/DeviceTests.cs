using Xunit;

namespace ConfigSync.Domain.Tests;

public class DeviceTests
{
    private static readonly byte[] Ciphertext = [0x01, 0x02, 0x03];

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        // given
        var credential = DeviceCredential.Password(Ciphertext);

        // when
        var given = new Device("router_01", "localhost", 2201, "device", credential);

        // then
        Assert.Equal("router_01", given.Name);
        Assert.Equal("localhost", given.Host);
        Assert.Equal(2201, given.Port);
        Assert.Equal("device", given.Username);
        Assert.Same(credential, given.Credential);
    }

    [Fact]
    public void AuthenticationType_ForPasswordCredential_IsPassword()
    {
        // given / when
        var given = new Device("01", "host", 22, "dev", DeviceCredential.Password(Ciphertext));

        // then
        Assert.Equal(DeviceAuthenticationType.Password, given.AuthenticationType);
    }

    [Fact]
    public void AuthenticationType_ForPrivateKeyCredential_IsPrivateKey()
    {
        // given / when
        var given = new Device("01", "host", 22, "dev", DeviceCredential.PrivateKey(Ciphertext));

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, given.AuthenticationType);
    }
}
