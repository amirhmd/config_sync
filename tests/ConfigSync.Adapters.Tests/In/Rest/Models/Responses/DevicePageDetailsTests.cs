using System.Collections.Generic;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class DevicePageDetailsTests
{
    [Fact]
    public void Construction_SetsItemsAndNextCursor()
    {
        // given
        var items = new List<DeviceDetails>
        {
            new("router_01", "localhost", 2201, "device", "password")
        };

        // when
        var page = new DevicePageDetails(items, "next-cursor");

        // then
        Assert.Single(page.Items);
        Assert.Equal("next-cursor", page.NextCursor);
    }

    [Fact]
    public void Construction_AllowsANullCursorOnTheLastPage()
    {
        // given / when
        var page = new DevicePageDetails([], null);

        // then
        Assert.Empty(page.Items);
        Assert.Null(page.NextCursor);
    }
}
