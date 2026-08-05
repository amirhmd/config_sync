using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class PageLimitValidation : IValidation<GetDevicesRequest>
{
    private const string PropertyName = "limit";
    private const int MinimumLimit = 1;
    private const int MaximumLimit = 100;

    public ValidationResult Validate(GetDevicesRequest request)
    {
        if (request.Limit is null)
        {
            return ValidationResult.Invalid(PropertyName, "Limit is required.");
        }

        if (request.Limit < MinimumLimit || request.Limit > MaximumLimit)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Limit must be between {MinimumLimit} and {MaximumLimit}.");
        }

        return ValidationResult.Valid;
    }
}
