using ConfigSync.Adapters.In.Rest.Models.Requests;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Requests;

public class CreateDeviceRequestTests
{
    [Fact]
    public void Construction_SetsEveryField()
    {
        // given / when
        var request = new CreateDeviceRequest(
            "router_01", "localhost", 2201, "device", "secret123", null);

        // then
        Assert.Equal("router_01", request.Name);
        Assert.Equal("localhost", request.Host);
        Assert.Equal(2201, request.Port);
        Assert.Equal("device", request.Username);
        Assert.Equal("secret123", request.Password);
        Assert.Null(request.PrivateKey);
    }

    [Fact]
    public void Construction_CarriesAPrivateKeyInsteadOfAPassword()
    {
        // given / when
        var request = new CreateDeviceRequest(
            "router_01", "localhost", 2201, "device", null, "-----BEGIN KEY-----");

        // then
        Assert.Null(request.Password);
        Assert.Equal("-----BEGIN KEY-----", request.PrivateKey);
    }
}
