using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class GetDeviceResponseTests
{
    private static DeviceDetails BuildDetails() =>
        new("router_01", "localhost", 2201, "device", "password");

    [Fact]
    public void Found_CarriesTheDevice()
    {
        // given
        var details = BuildDetails();

        // when
        var response = GetDeviceResponse.Found(details);

        // then
        Assert.Same(details, response.Device);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void NotFound_CarriesNeitherDeviceNorErrors()
    {
        // given / when
        var response = GetDeviceResponse.NotFound();

        // then
        Assert.Null(response.Device);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void Invalid_CarriesOnlyTheErrorDetails()
    {
        // given
        var errorDetails = new ErrorDetails([new ValidationError("name", "Name is required.")]);

        // when
        var response = GetDeviceResponse.Invalid(errorDetails);

        // then
        Assert.Null(response.Device);
        Assert.Same(errorDetails, response.ErrorDetails);
    }
}
