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
}
