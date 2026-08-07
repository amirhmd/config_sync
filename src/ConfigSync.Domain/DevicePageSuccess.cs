namespace ConfigSync.Domain;

public sealed class DevicePageSuccess(Page<Device> page) : DevicePageOutcome
{
    public Page<Device> Page { get; } = page;
}
