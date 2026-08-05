using System.Text.RegularExpressions;
using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed partial class DeviceNameValidation : IValidation<CreateDeviceRequest>
{
    private const string PropertyName = "name";

    public ValidationResult Validate(CreateDeviceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationResult.Invalid(PropertyName, "Name is required.");
        }

        if (!AllowedNamePattern().IsMatch(request.Name))
        {
            return ValidationResult.Invalid(
                PropertyName,
                "Name must be 1-64 characters, start with a lowercase letter or digit, and contain only lowercase letters, digits, hyphens, or underscores.");
        }

        return ValidationResult.Valid;
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9_-]{0,63}\z")]
    private static partial Regex AllowedNamePattern();
}
