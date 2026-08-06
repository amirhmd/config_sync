using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class NameValidationTests
{
    [Theory]
    [InlineData("router_01")]
    [InlineData("router-01")]
    [InlineData("a")]
    [InlineData("0device")]
    public void Validate_AcceptsWellFormedNames(string name)
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(name);

        // then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Router_01")]
    [InlineData("ROUTER")]
    public void Validate_RejectsUppercaseNames(string name)
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(name);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("name", Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData(" router_01")]
    [InlineData("router_01 ")]
    [InlineData("router 01")]
    [InlineData("router_01\n")]
    public void Validate_RejectsNamesCarryingWhitespace(string name)
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(name);

        // then
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("-router")]
    [InlineData("_router")]
    [InlineData("router/01")]
    [InlineData("router@01")]
    public void Validate_RejectsMalformedNames(string name)
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(name);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingName()
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(null);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNameLongerThan64Characters()
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate(new string('a', 65));

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ReportsOnlyOneErrorPerName()
    {
        // given
        var validation = new NameValidation();

        // when
        var result = validation.Validate("Router 01!");

        // then
        Assert.Single(result.Errors);
    }
}
