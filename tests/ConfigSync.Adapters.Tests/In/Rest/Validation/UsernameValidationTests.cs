using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class UsernameValidationTests
{
    [Fact]
    public void Validate_AcceptsAUsername()
    {
        // given
        var validation = new UsernameValidation();

        // when
        var result = validation.Validate("admin");

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingUsername()
    {
        // given
        var validation = new UsernameValidation();

        // when
        var result = validation.Validate("   ");

        // then
        Assert.False(result.IsValid);
        Assert.Equal("username", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsUsernameLongerThan64Characters()
    {
        // given
        var validation = new UsernameValidation();

        // when
        var result = validation.Validate(new string('a', 65));

        // then
        Assert.False(result.IsValid);
    }
}
