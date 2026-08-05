using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class DevicePortValidation : IValidation<CreateDeviceRequest>
{
    private const string PropertyName = "port";
    private const int MinimumPort = 1;
    private const int MaximumPort = 65535;

    public ValidationResult Validate(CreateDeviceRequest request)
    {
        if (request.Port is null)
        {
            return ValidationResult.Invalid(PropertyName, "Port is required.");
        }

        if (request.Port < MinimumPort || request.Port > MaximumPort)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Port must be between {MinimumPort} and {MaximumPort}.");
        }

        return ValidationResult.Valid;
    }
}
