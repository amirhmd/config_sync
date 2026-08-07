using System.Text.RegularExpressions;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed partial class NameValidation
{
    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !AllowedNamePattern().IsMatch(name))
        {
            return ValidationResult.Invalid(ValidationErrors.NameProperty, ValidationErrors.NameMessage);
        }

        return ValidationResult.Valid;
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9_-]{0,63}\z")]
    private static partial Regex AllowedNamePattern();
}
