namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class PortValidation
{
    public ValidationResult Validate(int? port)
    {
        if (port is null || port < ValidationErrors.MinimumPort || port > ValidationErrors.MaximumPort)
        {
            return ValidationResult.Invalid(ValidationErrors.PortProperty, ValidationErrors.PortMessage);
        }

        return ValidationResult.Valid;
    }
}
