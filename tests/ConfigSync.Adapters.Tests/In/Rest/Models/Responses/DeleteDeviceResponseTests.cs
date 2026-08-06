using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class DeleteDeviceResponseTests
{
    [Fact]
    public void Deleted_CarriesTheDeletedOutcome()
    {
        // given / when
        var response = DeleteDeviceResponse.Deleted();

        // then
        Assert.Equal(DeleteDeviceOutcome.Deleted, response.Outcome);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void NotFound_CarriesTheNotFoundOutcome()
    {
        // given / when
        var response = DeleteDeviceResponse.NotFound();

        // then
        Assert.Equal(DeleteDeviceOutcome.NotFound, response.Outcome);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public void Invalid_CarriesOnlyTheErrorDetails()
    {
        // given
        var errorDetails = new ErrorDetails([new ValidationError("name", "Name is required.")]);

        // when
        var response = DeleteDeviceResponse.Invalid(errorDetails);

        // then
        Assert.Null(response.Outcome);
        Assert.Same(errorDetails, response.ErrorDetails);
    }
}
