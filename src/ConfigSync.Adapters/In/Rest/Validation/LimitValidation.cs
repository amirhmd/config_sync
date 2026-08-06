namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class LimitValidation
{
    private const string PropertyName = "limit";
    private const int MinimumLimit = 1;
    private const int MaximumLimit = 100;

    public ValidationResult Validate(int? limit)
    {
        if (limit is null || limit < MinimumLimit || limit > MaximumLimit)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Limit is required and must be between {MinimumLimit} and {MaximumLimit}.");
        }

        return ValidationResult.Valid;
    }
}
