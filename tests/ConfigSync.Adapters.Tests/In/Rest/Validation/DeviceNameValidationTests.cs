using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class DeviceNameValidationTests
{
    [Theory]
    [InlineData("router_01")]
    [InlineData("router-01")]
    [InlineData("a")]
    [InlineData("0device")]
    public void Validate_AcceptsWellFormedNames(string name)
    {
        // given
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: name);

        // when
        var result = validation.Validate(request);

        // then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Router_01")]
    [InlineData("ROUTER")]
    public void Validate_RejectsUppercaseNames(string name)
    {
        // given
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: name);

        // when
        var result = validation.Validate(request);

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
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: name);

        // when
        var result = validation.Validate(request);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("name", Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData("-router")]
    [InlineData("_router")]
    [InlineData("router/01")]
    [InlineData("router@01")]
    public void Validate_RejectsMalformedNames(string name)
    {
        // given
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: name);

        // when
        var result = validation.Validate(request);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("name", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingName()
    {
        // given
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: "   ");

        // when
        var result = validation.Validate(request);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsNameLongerThan64Characters()
    {
        // given
        var validation = new DeviceNameValidation();
        var request = AdaptersTestFixtures.PasswordRequest(name: new string('a', 65));

        // when
        var result = validation.Validate(request);

        // then
        Assert.False(result.IsValid);
    }
}
