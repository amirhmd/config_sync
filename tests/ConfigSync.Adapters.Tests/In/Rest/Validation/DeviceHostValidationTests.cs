using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class DeviceHostValidationTests
{
    private static CreateDeviceRequest RequestWithHost(string? host) =>
        new("router_01", host, 2201, "device", "secret123", null);

    [Theory]
    [InlineData("localhost")]
    [InlineData("LOCALHOST")]
    [InlineData("192.168.1.20")]
    [InlineData("2001:db8::1")]
    [InlineData("device1.local")]
    [InlineData("router.example.com")]
    public void Validate_AcceptsSupportedHostForms(string host)
    {
        // given
        var validation = new DeviceHostValidation();

        // when
        var result = validation.Validate(RequestWithHost(host));

        // then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("ssh://device1")]
    [InlineData("device1:22")]
    [InlineData("user@device1")]
    [InlineData("device 1")]
    public void Validate_RejectsHostsCarryingExtraParts(string host)
    {
        // given
        var validation = new DeviceHostValidation();

        // when
        var result = validation.Validate(RequestWithHost(host));

        // then
        Assert.False(result.IsValid);
        Assert.Equal("host", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingHost()
    {
        // given
        var validation = new DeviceHostValidation();

        // when
        var result = validation.Validate(RequestWithHost("   "));

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsHostLongerThan253Characters()
    {
        // given
        var validation = new DeviceHostValidation();

        // when
        var result = validation.Validate(RequestWithHost(new string('a', 254)));

        // then
        Assert.False(result.IsValid);
    }
}
