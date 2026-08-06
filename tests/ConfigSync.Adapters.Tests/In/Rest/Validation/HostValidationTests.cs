using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class HostValidationTests
{
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
        var validation = new HostValidation();

        // when
        var result = validation.Validate(host);

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
        var validation = new HostValidation();

        // when
        var result = validation.Validate(host);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("host", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingHost()
    {
        // given
        var validation = new HostValidation();

        // when
        var result = validation.Validate(null);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsHostLongerThan253Characters()
    {
        // given
        var validation = new HostValidation();

        // when
        var result = validation.Validate(new string('a', 254));

        // then
        Assert.False(result.IsValid);
    }
}
