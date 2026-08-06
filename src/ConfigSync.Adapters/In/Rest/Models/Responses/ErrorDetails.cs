using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ConfigSync.Adapters.In.Rest.Validation;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed class ErrorDetails
{
    public ErrorDetails(IEnumerable<ValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = new ReadOnlyCollection<ValidationError>(new List<ValidationError>(errors));
    }

    public IReadOnlyList<ValidationError> Errors { get; }
}
