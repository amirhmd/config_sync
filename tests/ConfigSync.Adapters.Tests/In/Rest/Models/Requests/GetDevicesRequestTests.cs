using ConfigSync.Adapters.In.Rest.Models.Requests;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Requests;

public class GetDevicesRequestTests
{
    [Fact]
    public void Construction_SetsCursorAndLimit()
    {
        // given / when
        var request = new GetDevicesRequest("cursor-token", 50);

        // then
        Assert.Equal("cursor-token", request.Cursor);
        Assert.Equal(50, request.Limit);
    }

    [Fact]
    public void Construction_AllowsAnAbsentCursorForTheFirstPage()
    {
        // given / when
        var request = new GetDevicesRequest(null, 50);

        // then
        Assert.Null(request.Cursor);
    }

    [Fact]
    public void Construction_AllowsAnAbsentLimitSoValidationCanRejectIt()
    {
        // given / when
        var request = new GetDevicesRequest(null, null);

        // then
        Assert.Null(request.Limit);
    }
}
