using System.Collections.Generic;
using Xunit;

namespace ConfigSync.Domain.Tests;

public class PageTests
{
    [Fact]
    public void Constructor_SetsItemsAndNextCursor()
    {
        // given
        var items = new List<string> { "a", "b" };

        // when
        var given = new Page<string>(items, "cursor-token");

        // then
        Assert.Equal(2, given.Items.Count);
        Assert.Equal("cursor-token", given.NextCursor);
    }

    [Fact]
    public void NextCursor_CanBeNull_WhenLastPage()
    {
        // given / when
        var given = new Page<string>(new List<string> { "a" }, null);

        // then
        Assert.Null(given.NextCursor);
    }

    [Fact]
    public void EmptyPage_HasNoItemsAndNoCursor()
    {
        // given / when
        var given = new Page<string>([], null);

        // then
        Assert.Empty(given.Items);
        Assert.Null(given.NextCursor);
    }
}
