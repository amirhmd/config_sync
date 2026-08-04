using System;
using Xunit;

namespace ConfigSync.Domain.Tests;

public class ExecutionReferenceTests
{
    [Fact]
    public void Constructor_SetsReferenceId()
    {
        // given
        var id = Guid.NewGuid();

        // when
        var given = new ExecutionReference(id);

        // then
        Assert.Equal(id, given.ReferenceId);
    }

    [Fact]
    public void TwoReferencesWithSameId_AreEqual()
    {
        // given
        var id = Guid.NewGuid();

        // when
        var first = new ExecutionReference(id);
        var second = new ExecutionReference(id);

        // then
        Assert.Equal(first, second);
    }

    [Fact]
    public void TwoReferencesWithDifferentIds_AreNotEqual()
    {
        // given / when
        var first = new ExecutionReference(Guid.NewGuid());
        var second = new ExecutionReference(Guid.NewGuid());

        // then
        Assert.NotEqual(first, second);
    }
}
