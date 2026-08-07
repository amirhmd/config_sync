namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class UsernameValidation
{
    public ValidationResult Validate(string? username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > ValidationErrors.MaximumUsernameLength)
        {
            return ValidationResult.Invalid(ValidationErrors.UsernameProperty, ValidationErrors.UsernameMessage);
        }

        return ValidationResult.Valid;
    }
}
