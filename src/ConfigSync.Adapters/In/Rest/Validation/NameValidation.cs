using System.Text.RegularExpressions;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed partial class NameValidation
{
    private const string PropertyName = "name";

    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !AllowedNamePattern().IsMatch(name))
        {
            return ValidationResult.Invalid(
                PropertyName,
                "Name is required and must be 1-64 characters, start with a lowercase letter or digit, and contain only lowercase letters, digits, hyphens, or underscores.");
        }

        return ValidationResult.Valid;
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9_-]{0,63}\z")]
    private static partial Regex AllowedNamePattern();
}
