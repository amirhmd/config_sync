using System;
using System.Collections.Generic;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class ErrorDetailsTests
{
    [Fact]
    public void Construction_CarriesTheSuppliedErrors()
    {
        // given
        var errors = new[]
        {
            new ValidationError("name", "Name is required."),
            new ValidationError("port", "Port is required.")
        };

        // when
        var errorDetails = new ErrorDetails(errors);

        // then
        Assert.Equal(2, errorDetails.Errors.Count);
    }

    [Fact]
    public void Construction_CopiesTheSuppliedErrors()
    {
        // given
        var errors = new List<ValidationError>
        {
            new("name", "Name is required.")
        };
        var errorDetails = new ErrorDetails(errors);

        // when
        errors.Add(new ValidationError("port", "Port is required."));

        // then
        Assert.Single(errorDetails.Errors);
    }

    [Fact]
    public void Construction_ThrowsWhenErrorsAreNull()
    {
        // given / when / then
        Assert.Throws<ArgumentNullException>(() => new ErrorDetails(null!));
    }
}
