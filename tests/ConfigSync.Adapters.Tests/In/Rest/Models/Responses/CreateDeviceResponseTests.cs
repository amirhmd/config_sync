using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class CreateDeviceResponseTests
{
    private static DeviceDetails BuildDetails() =>
        new("router_01", "localhost", 2201, "device", "password");

    private static ErrorDetails BuildErrorDetails() =>
        new([new ValidationError("name", "Name is required.")]);

    [Fact]
    public void Created_CarriesTheDeviceAndCreatedOutcome()
    {
        // given
        var details = BuildDetails();

        // when
        var response = CreateDeviceResponse.Created(details);

        // then
        Assert.Equal(CreateDeviceOutcome.Created, response.Outcome);
        Assert.Same(details, response.Device);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void AlreadyExists_CarriesOnlyTheOutcome()
    {
        // given / when
        var response = CreateDeviceResponse.AlreadyExists();

        // then
        Assert.Equal(CreateDeviceOutcome.AlreadyExists, response.Outcome);
        Assert.Null(response.Device);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void Invalid_CarriesOnlyTheErrorDetails()
    {
        // given
        var errorDetails = BuildErrorDetails();

        // when
        var response = CreateDeviceResponse.Invalid(errorDetails);

        // then
        Assert.Null(response.Outcome);
        Assert.Null(response.Device);
        Assert.Same(errorDetails, response.ErrorDetails);
    }
}
