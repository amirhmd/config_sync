using Xunit;

namespace ConfigSync.Domain.Tests;

public class DeleteOutcomeTests
{
    [Fact]
    public void Deleted_HasDeletedValue()
    {
        Assert.Equal(OperationOutcome.Deleted, DeleteOutcome.Deleted.Value);
    }

    [Fact]
    public void NotFound_HasNotFoundValue()
    {
        Assert.Equal(OperationOutcome.NotFound, DeleteOutcome.NotFound.Value);
    }

    [Fact]
    public void StaticInstances_AreReferenceStable()
    {
        Assert.Same(DeleteOutcome.Deleted, DeleteOutcome.Deleted);
        Assert.Same(DeleteOutcome.NotFound, DeleteOutcome.NotFound);
    }

    [Fact]
    public void DifferentOutcomes_AreNotEqual()
    {
        Assert.NotEqual(DeleteOutcome.Deleted, DeleteOutcome.NotFound);
    }
}
