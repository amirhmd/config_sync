using ConfigSync.Adapters.In.Rest.Models.Responses;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class DeviceDetailsTests
{
    [Fact]
    public void Construction_SetsEveryField()
    {
        // given / when
        var details = new DeviceDetails("router_01", "localhost", 2201, "device", "password");

        // then
        Assert.Equal("router_01", details.Name);
        Assert.Equal("localhost", details.Host);
        Assert.Equal(2201, details.Port);
        Assert.Equal("device", details.Username);
        Assert.Equal("password", details.AuthenticationType);
    }
}
