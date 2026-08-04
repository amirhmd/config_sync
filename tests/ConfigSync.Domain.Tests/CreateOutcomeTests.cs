using Xunit;

namespace ConfigSync.Domain.Tests;

public class CreateOutcomeTests
{
    [Fact]
    public void Created_HasCreatedValue()
    {
        Assert.Equal(OperationOutcome.Created, CreateOutcome.Created.Value);
    }

    [Fact]
    public void AlreadyExists_HasAlreadyExistsValue()
    {
        Assert.Equal(OperationOutcome.AlreadyExists, CreateOutcome.AlreadyExists.Value);
    }

    [Fact]
    public void StaticInstances_AreReferenceStable()
    {
        Assert.Same(CreateOutcome.Created, CreateOutcome.Created);
        Assert.Same(CreateOutcome.AlreadyExists, CreateOutcome.AlreadyExists);
    }

    [Fact]
    public void DifferentOutcomes_AreNotEqual()
    {
        Assert.NotEqual(CreateOutcome.Created, CreateOutcome.AlreadyExists);
    }
}
