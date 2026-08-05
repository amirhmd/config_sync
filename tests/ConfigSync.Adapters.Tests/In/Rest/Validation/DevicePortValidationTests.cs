using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class DevicePortValidationTests
{
    private static CreateDeviceRequest RequestWithPort(int? port) =>
        new("router_01", "localhost", port, "device", "secret123", null);

    [Theory]
    [InlineData(1)]
    [InlineData(22)]
    [InlineData(65535)]
    public void Validate_AcceptsPortsInRange(int port)
    {
        // given
        var validation = new DevicePortValidation();

        // when
        var result = validation.Validate(RequestWithPort(port));

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
        var validation = new DevicePortValidation();

        // when
        var result = validation.Validate(RequestWithPort(port));

        // then
        Assert.False(result.IsValid);
        Assert.Equal("port", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void Validate_RejectsMissingPort()
    {
        // given
        var validation = new DevicePortValidation();

        // when
        var result = validation.Validate(RequestWithPort(null));

        // then
        Assert.False(result.IsValid);
    }
}
