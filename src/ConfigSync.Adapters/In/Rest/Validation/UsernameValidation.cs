namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class UsernameValidation
{
    private const string PropertyName = "username";
    private const int MaximumUsernameLength = 64;

    public ValidationResult Validate(string? username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > MaximumUsernameLength)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Username is required and cannot exceed {MaximumUsernameLength} characters.");
        }

        return ValidationResult.Valid;
    }
}
