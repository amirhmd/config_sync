using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class PortValidationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(22)]
    [InlineData(65535)]
    public void Validate_AcceptsPortsInRange(int port)
    {
        // given
        var validation = new PortValidation();

        // when
        var result = validation.Validate(port);

        // then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void Validate_RejectsPortsOutOfRange(int port)
    {
        // given
        var validation = new PortValidation();

        // when
        var result = validation.Validate(port);

        // then
        Assert.False(result.IsValid);
        Assert.Equal("port", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingPort()
    {
        // given
        var validation = new PortValidation();

        // when
        var result = validation.Validate(null);

        // then
        Assert.False(result.IsValid);
    }
}
