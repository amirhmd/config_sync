using System.Collections.Immutable;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public class ExecutionPlanMapper : IExecutionPlanMapper
{
    public ExecutionPlan ToDomain(ExecuteCommandRequest request) =>
        new(
            Commands: request.Commands.ToImmutableList(),
            DeviceIds: request.DeviceIds.ToImmutableList());
}
