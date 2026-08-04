namespace ConfigSync.Adapters.In.Rest;

internal static class DeviceName
{
    public static string Normalize(string name) => name.Trim().ToLowerInvariant();
}
