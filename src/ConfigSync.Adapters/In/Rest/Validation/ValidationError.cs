using System;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class ValidationError
{
    public ValidationError(string propertyName, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        PropertyName = propertyName;
        Message = message;
    }

    public string PropertyName { get; }

    public string Message { get; }
}
