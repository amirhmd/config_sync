using System.Collections.Generic;
using System.Linq;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using Microsoft.Extensions.Logging;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class RequestValidation(
    NameValidation nameValidation,
    HostValidation hostValidation,
    PortValidation portValidation,
    UsernameValidation usernameValidation,
    CredentialValidation credentialValidation,
    LimitValidation limitValidation,
    ILogger<RequestValidation> logger)
{
    public ErrorDetails? Validate(CreateDeviceRequest request)
    {
        var errors = new List<ValidationError>();

        Collect(errors, nameValidation.Validate(request.Name));
        Collect(errors, hostValidation.Validate(request.Host));
        Collect(errors, portValidation.Validate(request.Port));
        Collect(errors, usernameValidation.Validate(request.Username));
        Collect(errors, credentialValidation.Validate(request.Password, request.PrivateKey));

        return ToErrorDetails(nameof(CreateDeviceRequest), errors);
    }

    public ErrorDetails? Validate(GetDeviceRequest request)
    {
        var errors = new List<ValidationError>();

        Collect(errors, nameValidation.Validate(request.Name));

        return ToErrorDetails(nameof(GetDeviceRequest), errors);
    }

    public ErrorDetails? Validate(DeleteDeviceRequest request)
    {
        var errors = new List<ValidationError>();

        Collect(errors, nameValidation.Validate(request.Name));

        return ToErrorDetails(nameof(DeleteDeviceRequest), errors);
    }

    public ErrorDetails? Validate(GetDevicesRequest request)
    {
        var errors = new List<ValidationError>();

        Collect(errors, limitValidation.Validate(request.Limit));

        return ToErrorDetails(nameof(GetDevicesRequest), errors);
    }

    private static void Collect(List<ValidationError> errors, ValidationResult result)
    {
        if (result.IsValid)
        {
            return;
        }

        errors.AddRange(result.Errors);
    }

    private ErrorDetails? ToErrorDetails(string requestType, List<ValidationError> errors)
    {
        if (errors.Count == 0)
        {
            return null;
        }

        var rejectedProperties = errors.Select(error => error.PropertyName).ToArray();

        logger.LogWarning(
            "Rejected {RequestType} because {RejectedPropertyCount} properties failed validation: {RejectedProperties}",
            requestType,
            rejectedProperties.Length,
            rejectedProperties);

        return new ErrorDetails(errors);
    }
}
