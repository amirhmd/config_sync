using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class DeviceUsernameValidationTests
{
    private static CreateDeviceRequest RequestWithUsername(string? username) =>
        new("router_01", "localhost", 2201, username, "secret123", null);

    [Fact]
    public void Validate_AcceptsAUsername()
    {
        // given
        var validation = new DeviceUsernameValidation();

        // when
        var result = validation.Validate(RequestWithUsername("admin"));

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingUsername()
    {
        // given
        var validation = new DeviceUsernameValidation();

        // when
        var result = validation.Validate(RequestWithUsername("   "));

        // then
        Assert.False(result.IsValid);
        Assert.Equal("username", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsUsernameLongerThan64Characters()
    {
        // given
        var validation = new DeviceUsernameValidation();

        // when
        var result = validation.Validate(RequestWithUsername(new string('a', 65)));

        // then
        Assert.False(result.IsValid);
    }
}
