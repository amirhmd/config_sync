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
            Password: null,
            PrivateKey: "some-key-content");

        // then
        Assert.Equal("device123", given.Id);
        Assert.Equal("localhost", given.Host);
        Assert.Equal(2201, given.Port);
        Assert.Equal("device", given.Username);
        Assert.Null(given.Password);
        Assert.Equal("some-key-content", given.PrivateKey);
    }

    [Fact]
    public void TwoDevicesWithSameValues_AreEqual()
    {
        // given
        var first = new Device("device123", "localhost", 2201, "device", null, "key");
        var second = new Device("device123", "localhost", 2201, "device", null, "key");

        // then
        Assert.Equal(first, second);
    }
}