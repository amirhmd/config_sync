using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class LimitValidationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_AcceptsLimitsInRange(int limit)
    {
        // given
        var validation = new LimitValidation();

        // when
        var result = validation.Validate(limit);

        // then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_RejectsLimitsOutOfRange(int limit)
    {
        // given
        var validation = new LimitValidation();

        // when
        var result = validation.Validate(limit);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("limit", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingLimit()
    {
        // given
        var validation = new LimitValidation();

        // when
        var result = validation.Validate(null);

        // then
        Assert.False(result.IsValid);
    }
}
