using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class DeviceUsernameValidation : IValidation<CreateDeviceRequest>
{
    private const string PropertyName = "username";
    private const int MaximumUsernameLength = 64;

    public ValidationResult Validate(CreateDeviceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return ValidationResult.Invalid(PropertyName, "Username is required.");
        }

        if (request.Username.Length > MaximumUsernameLength)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Username cannot exceed {MaximumUsernameLength} characters.");
        }

        return ValidationResult.Valid;
    }
}
