using ConfigSync.Adapters.In.Rest.Models.Requests;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Requests;

public class GetDeviceRequestTests
{
    [Fact]
    public void Construction_SetsTheName()
    {
        // given / when
        var request = new GetDeviceRequest("router_01");

        // then
        Assert.Equal("router_01", request.Name);
    }

    [Fact]
    public void Construction_AllowsAnAbsentName()
    {
        // given / when
        var request = new GetDeviceRequest(null);

        // then
        Assert.Null(request.Name);
    }
}
