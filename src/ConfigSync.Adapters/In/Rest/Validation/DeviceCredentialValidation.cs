using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class DeviceCredentialValidation : IValidation<CreateDeviceRequest>
{
    private const string PropertyName = "credential";

    // Presence only
    public ValidationResult Validate(CreateDeviceRequest request)
    {
        var hasPassword = !string.IsNullOrWhiteSpace(request.Password);
        var hasPrivateKey = !string.IsNullOrWhiteSpace(request.PrivateKey);

        if (hasPassword && hasPrivateKey)
        {
            return ValidationResult.Invalid(
                PropertyName,
                "Provide either a password or a private key, not both.");
        }

        if (!hasPassword && !hasPrivateKey)
        {
            return ValidationResult.Invalid(
                PropertyName,
                "Either a password or a private key is required.");
        }

        return ValidationResult.Valid;
    }
}
