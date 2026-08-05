using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class ValidationResult
{
    public static ValidationResult Valid { get; } = new(Array.Empty<ValidationError>());

    private ValidationResult(IEnumerable<ValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = new ReadOnlyCollection<ValidationError>(new List<ValidationError>(errors));
    }

    public IReadOnlyList<ValidationError> Errors { get; }

    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Invalid(string propertyName, string message)
    {
        var error = new ValidationError(propertyName, message);
        return new ValidationResult([error]);
    }
}
