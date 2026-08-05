using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class ValidationResultTests
{
    [Fact]
    public void Valid_HasNoErrors()
    {
        // given / when / then
        Assert.True(ValidationResult.Valid.IsValid);
        Assert.Empty(ValidationResult.Valid.Errors);
    }

    [Fact]
    public void Invalid_CarriesTheSuppliedError()
    {
        // given / when
        var result = ValidationResult.Invalid("port", "Port is required.");

        // then
        var error = Assert.Single(result.Errors);
        Assert.Equal("port", error.PropertyName);
        Assert.Equal("Port is required.", error.Message);
    }

    [Fact]
    public void Invalid_IsNotValid()
    {
        // given / when
        var result = ValidationResult.Invalid("port", "Port is required.");

        // then
        Assert.False(result.IsValid);
    }
}
