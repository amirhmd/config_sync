namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class CredentialValidation
{
    public ValidationResult Validate(string? password, string? privateKey)
    {
        var hasPassword = !string.IsNullOrWhiteSpace(password);
        var hasPrivateKey = !string.IsNullOrWhiteSpace(privateKey);

        if (hasPassword == hasPrivateKey)
        {
            return ValidationResult.Invalid(ValidationErrors.CredentialProperty, ValidationErrors.CredentialMessage);
        }

        return ValidationResult.Valid;
    }
}
