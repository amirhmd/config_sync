namespace ConfigSync.Domain;

public sealed class DevicePageInvalidCursor : DevicePageOutcome
{
    public static DevicePageInvalidCursor Instance { get; } = new();

    private DevicePageInvalidCursor()
    {
        // not instantiable 
    }
}
