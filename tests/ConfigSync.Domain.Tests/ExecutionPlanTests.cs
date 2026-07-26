using System.Collections.Immutable;
using Xunit;

namespace ConfigSync.Domain.Tests;

public class ExecutionPlanTests
{
    [Fact]
    public void Constructor_SetsCommandsAndDeviceIds()
    {
        // given
        var given = new ExecutionPlan(
            Commands: ImmutableList.Create("show version", "show interfaces"),
            DeviceIds: ImmutableList.Create("device123", "device456"));

        // then
        Assert.Equal(2, given.Commands.Count);
        Assert.Equal(2, given.DeviceIds.Count);
        Assert.Contains("show version", given.Commands);
        Assert.Contains("device123", given.DeviceIds);
    }

    [Fact]
    public void Add_OnCommands_DoesNotMutateOriginalList()
    {
        // given
        var given = new ExecutionPlan(
            Commands: ImmutableList.Create("show version"),
            DeviceIds: ImmutableList.Create("device123"));

        // when
        var withExtraCommand = given.Commands.Add("show clock");

        // then
        Assert.Single(given.Commands);
        Assert.Equal(2, withExtraCommand.Count);
    }
}