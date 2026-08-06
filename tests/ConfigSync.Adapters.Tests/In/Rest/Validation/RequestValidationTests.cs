using System.Linq;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Validation;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class RequestValidationTests
{
    private static RequestValidation BuildValidation() =>
        new(
            new NameValidation(),
            new HostValidation(),
            new PortValidation(),
            new UsernameValidation(),
            new CredentialValidation(),
            new LimitValidation(),
            NullLogger<RequestValidation>.Instance);

    [Fact]
    public void Validate_CreateDeviceRequest_ReturnsNullWhenEveryFieldIsValid()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(AdaptersTestConfiguration.PasswordRequest());

        // then
        Assert.Null(errorDetails);
    }

    [Fact]
    public void Validate_CreateDeviceRequest_ReportsEveryInvalidFieldIndependently()
    {
        // given
        var validation = BuildValidation();
        var request = new CreateDeviceRequest(
            Name: "Router 01",
            Host: "ssh://device1",
            Port: 0,
            Username: "   ",
            Password: null,
            PrivateKey: null);

        // when
        var errorDetails = validation.Validate(request);

        // then
        Assert.NotNull(errorDetails);
        var reportedProperties = errorDetails.Errors.Select(error => error.PropertyName).ToList();
        Assert.Contains("name", reportedProperties);
        Assert.Contains("host", reportedProperties);
        Assert.Contains("port", reportedProperties);
        Assert.Contains("username", reportedProperties);
        Assert.Contains("credential", reportedProperties);
    }

    [Fact]
    public void Validate_CreateDeviceRequest_RejectsBothCredentials()
    {
        // given
        var validation = BuildValidation();
        var request = new CreateDeviceRequest(
            "router_01", "localhost", 2201, "device", "secret123", "-----BEGIN KEY-----");

        // when
        var errorDetails = validation.Validate(request);

        // then
        Assert.NotNull(errorDetails);
        Assert.Equal("credential", Assert.Single(errorDetails.Errors).PropertyName);
    }

    [Fact]
    public void Validate_GetDeviceRequest_ReturnsNullForAValidName()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(new GetDeviceRequest("router_01"));

        // then
        Assert.Null(errorDetails);
    }

    [Fact]
    public void Validate_GetDeviceRequest_RejectsAMalformedName()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(new GetDeviceRequest("Router 01"));

        // then
        Assert.NotNull(errorDetails);
        Assert.Equal("name", Assert.Single(errorDetails.Errors).PropertyName);
    }

    [Fact]
    public void Validate_DeleteDeviceRequest_RejectsAMalformedName()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(new DeleteDeviceRequest("Router 01"));

        // then
        Assert.NotNull(errorDetails);
        Assert.Equal("name", Assert.Single(errorDetails.Errors).PropertyName);
    }

    [Fact]
    public void Validate_GetDevicesRequest_ReturnsNullForAValidLimit()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(new GetDevicesRequest(null, 50));

        // then
        Assert.Null(errorDetails);
    }

    [Fact]
    public void Validate_GetDevicesRequest_RejectsALimitOutOfRange()
    {
        // given
        var validation = BuildValidation();

        // when
        var errorDetails = validation.Validate(new GetDevicesRequest(null, 101));

        // then
        Assert.NotNull(errorDetails);
        Assert.Equal("limit", Assert.Single(errorDetails.Errors).PropertyName);
    }
}
