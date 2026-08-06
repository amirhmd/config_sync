namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class CredentialValidation
{
    private const string PropertyName = "credential";

    public ValidationResult Validate(string? password, string? privateKey)
    {
        var hasPassword = !string.IsNullOrWhiteSpace(password);
        var hasPrivateKey = !string.IsNullOrWhiteSpace(privateKey);

        if (hasPassword == hasPrivateKey)
        {
            return ValidationResult.Invalid(
                PropertyName,
                "Provide exactly one of password or private key.");
        }

        return ValidationResult.Valid;
    }
}
