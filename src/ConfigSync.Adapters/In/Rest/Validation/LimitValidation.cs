namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class LimitValidation
{
    public ValidationResult Validate(int? limit)
    {
        if (limit is null || limit < ValidationErrors.MinimumLimit || limit > ValidationErrors.MaximumLimit)
        {
            return ValidationResult.Invalid(ValidationErrors.LimitProperty, ValidationErrors.LimitMessage);
        }

        return ValidationResult.Valid;
    }
}
