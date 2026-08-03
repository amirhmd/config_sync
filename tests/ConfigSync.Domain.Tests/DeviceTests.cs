using Xunit;

namespace ConfigSync.Domain.Tests;

public class DeviceTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        // given
        var given = new Device(
            Id: "device123",
            Host: "localhost",
            Port: 2201,
            Username: "device",
            AuthenticationType: DeviceAuthenticationType.PrivateKey,
            Version: 1);

        // then
        Assert.Equal("device123", given.Id);
        Assert.Equal("localhost", given.Host);
        Assert.Equal(2201, given.Port);
        Assert.Equal("device", given.Username);
        Assert.Equal(DeviceAuthenticationType.PrivateKey, given.AuthenticationType);
        Assert.Equal(1, given.Version);
    }

    [Fact]
    public void TwoDevicesWithSameValues_AreEqual()
    {
        // given
        var first = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.Password, 1);
        var second = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.Password, 1);

        // then
        Assert.Equal(first, second);
    }

    [Fact]
    public void TwoDevicesWithDifferentVersions_AreNotEqual()
    {
        // given
        var first = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.Password, 1);
        var second = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.Password, 2);

        // then
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void TwoDevicesWithDifferentAuthenticationTypes_AreNotEqual()
    {
        // given
        var first = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.Password, 1);
        var second = new Device(
            "device123", "localhost", 2201, "device",
            DeviceAuthenticationType.PrivateKey, 1);

        // then
        Assert.NotEqual(first, second);
    }
}
