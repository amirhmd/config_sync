using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class CredentialValidationTests
{
    [Fact]
    public void Validate_AcceptsPasswordOnly()
    {
        // given
        var validation = new CredentialValidation();

        // when
        var result = validation.Validate("secret123", null);

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_AcceptsPrivateKeyOnly()
    {
        // given
        var validation = new CredentialValidation();

        // when
        var result = validation.Validate(null, "-----BEGIN KEY-----");

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsBothCredentials()
    {
        // given
        var validation = new CredentialValidation();

        // when
        var result = validation.Validate("secret123", "-----BEGIN KEY-----");

        // then
        Assert.False(result.IsValid);
        Assert.Equal("credential", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsNeitherCredential()
    {
        // given
        var validation = new CredentialValidation();

        // when
        var result = validation.Validate(null, null);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("credential", Assert.Single(result.Errors).PropertyName);
    }
}
