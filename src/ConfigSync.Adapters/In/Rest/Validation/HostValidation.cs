using System;
using System.Net;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class HostValidation
{
    public ValidationResult Validate(string? host)
    {
        if (IsAcceptable(host))
        {
            return ValidationResult.Valid;
        }

        return ValidationResult.Invalid(ValidationErrors.HostProperty, ValidationErrors.HostMessage);
    }

    private static bool IsAcceptable(string? host)
    {
        if (string.IsNullOrWhiteSpace(host) || host.Length > ValidationErrors.MaximumHostLength)
        {
            return false;
        }

        return IsLocalhost(host) || IsIpAddress(host) || IsDnsName(host);
    }

    private static bool IsLocalhost(string host)
    {
        return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsIpAddress(string host)
    {
        return IPAddress.TryParse(host, out _);
    }

    private static bool IsDnsName(string host)
    {
        return Uri.CheckHostName(host) == UriHostNameType.Dns;
    }
}
