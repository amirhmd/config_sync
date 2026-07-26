using ConfigSync.Adapters.In.Rest.Models;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models;

public class ExecuteCommandRequestTests
{
    [Fact]
    public void Construction_SetsCommandsAndDeviceIds()
    {
        // given
        var given = new ExecuteCommandRequest
        {
            Commands = ["show version"],
            DeviceIds = ["device1"]
        };

        // then
        Assert.Single(given.Commands);
        Assert.Single(given.DeviceIds);
    }

    [Fact]
    public void Construction_DefaultsToEmptyLists()
    {
        // given
        var given = new ExecuteCommandRequest();

        // then
        Assert.Empty(given.Commands);
        Assert.Empty(given.DeviceIds);
    }
}