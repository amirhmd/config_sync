using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Mappers;

public class ExecutionPlanMapperTests
{
    [Fact]
    public void ToDomain_MapsCommandsAndDeviceIds()
    {
        // given
        IExecutionPlanMapper given = new ExecutionPlanMapper();
        var request = new ExecuteCommandRequest
        {
            Commands = ["show version", "show interfaces"],
            DeviceIds = ["device1", "device2"]
        };

        // when
        var plan = given.ToDomain(request);

        // then
        Assert.Equal(2, plan.Commands.Count);
        Assert.Equal(2, plan.DeviceIds.Count);
        Assert.Contains("show version", plan.Commands);
        Assert.Contains("device1", plan.DeviceIds);
    }
}
