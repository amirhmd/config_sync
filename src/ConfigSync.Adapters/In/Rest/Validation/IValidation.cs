namespace ConfigSync.Adapters.In.Rest.Validation;

public interface IValidation<in TRequest>
{
    /// <summary>
    /// Checks one concern of the request. Returns <see cref="ValidationResult.Valid"/> when the
    /// concern is satisfied; otherwise the errors the endpoint aggregates.
    /// </summary>
    ValidationResult Validate(TRequest request);
}
