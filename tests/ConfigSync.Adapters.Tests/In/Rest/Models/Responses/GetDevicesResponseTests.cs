using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class GetDevicesResponseTests
{
    [Fact]
    public void Success_CarriesThePage()
    {
        // given
        var page = new DevicePageDetails([], null);

        // when
        var response = GetDevicesResponse.Success(page);

        // then
        Assert.Same(page, response.Page);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void Invalid_CarriesOnlyTheErrorDetails()
    {
        // given
        var errorDetails = new ErrorDetails([new ValidationError("limit", "Limit is required.")]);

        // when
        var response = GetDevicesResponse.Invalid(errorDetails);

        // then
        Assert.Null(response.Page);
        Assert.Same(errorDetails, response.ErrorDetails);
    }
}
