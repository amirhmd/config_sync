using System;
using System.Net;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class HostValidation
{
    private const string PropertyName = "host";
    private const int MaximumHostLength = 253;

    public ValidationResult Validate(string? host)
    {
        if (IsAcceptable(host))
        {
            return ValidationResult.Valid;
        }

        return ValidationResult.Invalid(
            PropertyName,
            $"Host is required, cannot exceed {MaximumHostLength} characters, and must be a DNS hostname, an IPv4 address, an IPv6 address, or localhost.");
    }

    private static bool IsAcceptable(string? host)
    {
        if (string.IsNullOrWhiteSpace(host) || host.Length > MaximumHostLength)
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
