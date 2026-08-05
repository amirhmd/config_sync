using System.Collections.Generic;
using System.Linq;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using ConfigSync.Application;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class CreateDeviceValidationCoverageTests
{
    private static IReadOnlyList<ValidationError> ValidateWithEveryRegisteredValidation(
        CreateDeviceRequest request)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        return provider
            .GetRequiredService<IEnumerable<IValidation<CreateDeviceRequest>>>()
            .SelectMany(validation => validation.Validate(request).Errors)
            .ToList();
    }

    [Fact]
    public void EveryInvalidField_ProducesItsOwnError()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "Router 01",
            Host: "ssh://device1",
            Port: 0,
            Username: "   ",
            Password: null,
            PrivateKey: null);

        // when
        var errors = ValidateWithEveryRegisteredValidation(request);

        // then 
        var reportedProperties = errors.Select(error => error.PropertyName).ToList();
        Assert.Contains("name", reportedProperties);
        Assert.Contains("host", reportedProperties);
        Assert.Contains("port", reportedProperties);
        Assert.Contains("username", reportedProperties);
        Assert.Contains("credential", reportedProperties);
    }

    [Fact]
    public void EveryMissingField_ProducesItsOwnError()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: null,
            Host: null,
            Port: null,
            Username: null,
            Password: null,
            PrivateKey: null);

        // when
        var errors = ValidateWithEveryRegisteredValidation(request);

        // then
        var reportedProperties = errors.Select(error => error.PropertyName).ToList();
        Assert.Contains("name", reportedProperties);
        Assert.Contains("host", reportedProperties);
        Assert.Contains("port", reportedProperties);
        Assert.Contains("username", reportedProperties);
        Assert.Contains("credential", reportedProperties);
    }

    [Fact]
    public void AFullyValidRequest_ProducesNoErrors()
    {
        // given
        var request = AdaptersTestFixtures.PasswordRequest();

        // when
        var errors = ValidateWithEveryRegisteredValidation(request);

        // then
        Assert.Empty(errors);
    }
}
