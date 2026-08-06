namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class PortValidation
{
    private const string PropertyName = "port";
    private const int MinimumPort = 1;
    private const int MaximumPort = 65535;

    public ValidationResult Validate(int? port)
    {
        if (port is null || port < MinimumPort || port > MaximumPort)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Port is required and must be between {MinimumPort} and {MaximumPort}.");
        }

        return ValidationResult.Valid;
    }
}
