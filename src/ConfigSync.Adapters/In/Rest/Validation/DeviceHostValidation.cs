using System;
using System.Net;
using ConfigSync.Adapters.In.Rest.Models.Requests;

namespace ConfigSync.Adapters.In.Rest.Validation;

public sealed class DeviceHostValidation : IValidation<CreateDeviceRequest>
{
    private const string PropertyName = "host";
    private const int MaximumHostLength = 253;

    public ValidationResult Validate(CreateDeviceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Host))
        {
            return ValidationResult.Invalid(PropertyName, "Host is required.");
        }

        if (request.Host.Length > MaximumHostLength)
        {
            return ValidationResult.Invalid(
                PropertyName,
                $"Host cannot exceed {MaximumHostLength} characters.");
        }

        if (IsLocalhost(request.Host) || IsIpAddress(request.Host) || IsDnsName(request.Host))
        {
            return ValidationResult.Valid;
        }

        return ValidationResult.Invalid(
            PropertyName,
            "Host must be a DNS hostname, an IPv4 address, an IPv6 address, or localhost.");
    }

    private static bool IsLocalhost(string host) =>
        string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase);

    private static bool IsIpAddress(string host) =>
        IPAddress.TryParse(host, out _);

    private static bool IsDnsName(string host) =>
        Uri.CheckHostName(host) == UriHostNameType.Dns;
}
