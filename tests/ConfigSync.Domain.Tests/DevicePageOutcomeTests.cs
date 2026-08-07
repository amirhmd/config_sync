using Xunit;

namespace ConfigSync.Domain.Tests;

public class DevicePageOutcomeTests
{
    [Fact]
    public void Success_CarriesThePage()
    {
        // given
        var page = new Page<Device>([], null);

        // when
        var outcome = DevicePageOutcome.Success(page);

        // then
        Assert.False(outcome.CursorIsInvalid);
        Assert.Same(page, outcome.Page);
    }

    [Fact]
    public void InvalidCursor_CarriesAnEmptyPage()
    {
        // given / when
        var outcome = DevicePageOutcome.InvalidCursor();

        // then
        Assert.True(outcome.CursorIsInvalid);
        Assert.Empty(outcome.Page.Items);
        Assert.Null(outcome.Page.NextCursor);
    }
}