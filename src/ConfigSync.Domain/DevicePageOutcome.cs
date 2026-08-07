namespace ConfigSync.Domain;

public sealed class DevicePageOutcome
{
    public Page<Device> Page { get; }
    public bool CursorIsInvalid { get; }

    private DevicePageOutcome(Page<Device> page, bool cursorIsInvalid)
    {
        Page = page;
        CursorIsInvalid = cursorIsInvalid;
    }

    public static DevicePageOutcome Success(Page<Device> page) => new(page, false);

    public static DevicePageOutcome InvalidCursor() => new(new Page<Device>([], null), true);
}