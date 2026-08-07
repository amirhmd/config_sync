namespace ConfigSync.Adapters.In.Rest.Validation;

internal static class ValidationErrors
{
    internal const int MinimumPort = 1;
    internal const int MaximumPort = 65535;
    internal const int MinimumLimit = 1;
    internal const int MaximumLimit = 100;
    internal const int MaximumHostLength = 253;
    internal const int MaximumUsernameLength = 64;

    internal const string NameProperty = "name";
    internal const string HostProperty = "host";
    internal const string PortProperty = "port";
    internal const string UsernameProperty = "username";
    internal const string CredentialProperty = "credential";
    internal const string LimitProperty = "limit";
    internal const string CursorProperty = "cursor";

    internal const string NameMessage =
        "Name is required and must be 1-64 characters, start with a lowercase letter or digit, "
        + "and contain only lowercase letters, digits, hyphens, or underscores.";

    internal static readonly string HostMessage =
        $"Host is required, cannot exceed {MaximumHostLength} characters, and must be a DNS "
        + "hostname, an IPv4 address, an IPv6 address, or localhost.";

    internal static readonly string PortMessage = $"Port is required and must be between {MinimumPort} and {MaximumPort}.";

    internal static readonly string UsernameMessage = $"Username is required and cannot exceed {MaximumUsernameLength} characters.";

    internal const string CredentialMessage = "Provide exactly one of password or private key.";

    internal static readonly string LimitMessage = $"Limit is required and must be between {MinimumLimit} and {MaximumLimit}.";

    internal const string CursorMessage = "Cursor is invalid";
}
