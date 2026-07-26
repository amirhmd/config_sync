using ConfigSync.Adapters.Out.Persistence.Entities;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Entities;

public class DeviceEntityTests
{
    [Fact]
    public void Construction_SetsRequiredAndOptionalProperties()
    {
        // given
        var given = new DeviceEntity
        {
            Id = "device123",
            Host = "localhost",
            Port = 2201,
            Username = "device",
            Password = null,
            PrivateKey = "some-key-content"
        };

        // then
        Assert.Equal("device123", given.Id);
        Assert.Equal("localhost", given.Host);
        Assert.Equal(2201, given.Port);
        Assert.Equal("device", given.Username);
        Assert.Null(given.Password);
        Assert.Equal("some-key-content", given.PrivateKey);
    }
}