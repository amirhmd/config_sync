using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class DeviceCredentialValidationTests
{
    private static CreateDeviceRequest RequestWithCredentials(string? password, string? privateKey) =>
        new("router_01", "localhost", 2201, "device", password, privateKey);

    [Fact]
    public void Validate_AcceptsPasswordOnly()
    {
        // given
        var validation = new DeviceCredentialValidation();

        // when
        var result = validation.Validate(RequestWithCredentials("secret123", null));

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_AcceptsPrivateKeyOnly()
    {
        // given
        var validation = new DeviceCredentialValidation();

        // when
        var result = validation.Validate(RequestWithCredentials(null, "-----BEGIN KEY-----"));

        // then
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsBothCredentials()
    {
        // given
        var validation = new DeviceCredentialValidation();

        // when
        var result = validation.Validate(RequestWithCredentials("secret123", "-----BEGIN KEY-----"));

        // then
        Assert.False(result.IsValid);
        Assert.Equal("credential", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsNeitherCredential()
    {
        // given
        var validation = new DeviceCredentialValidation();

        // when
        var result = validation.Validate(RequestWithCredentials(null, null));

        // then
        Assert.False(result.IsValid);
        Assert.Equal("credential", Assert.Single(result.Errors).PropertyName);
    }
}
