using Xunit;

namespace ConfigSync.Domain.Tests;

public class DevicePageOutcomeTests
{
    [Fact]
    public void DevicePageSuccess_CarriesThePage()
    {
        // given
        var page = new Page<Device>([], null);

        // when
        var outcome = new DevicePageSuccess(page);

        // then
        Assert.Same(page, outcome.Page);
    }

    [Fact]
    public void DevicePageSuccess_IsADevicePageOutcome()
    {
        // given / when
        DevicePageOutcome outcome = new DevicePageSuccess(new Page<Device>([], null));

        // then
        Assert.IsType<DevicePageSuccess>(outcome);
    }

    [Fact]
    public void DevicePageInvalidCursor_IsADevicePageOutcome()
    {
        // given / when
        DevicePageOutcome outcome = DevicePageInvalidCursor.Instance;

        // then
        Assert.IsType<DevicePageInvalidCursor>(outcome);
    }

    [Fact]
    public void DevicePageInvalidCursor_ReusesASingleInstance()
    {
        // given / when / then
        Assert.Same(DevicePageInvalidCursor.Instance, DevicePageInvalidCursor.Instance);
    }
}
