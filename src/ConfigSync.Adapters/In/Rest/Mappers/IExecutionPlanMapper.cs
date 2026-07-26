using ConfigSync.Adapters.In.Rest.Models;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public interface IExecutionPlanMapper
{
    ExecutionPlan ToDomain(ExecuteCommandRequest request);
}