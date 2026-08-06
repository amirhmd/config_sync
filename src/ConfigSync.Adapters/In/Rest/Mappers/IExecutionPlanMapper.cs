using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public interface IExecutionPlanMapper
{
    ExecutionPlan ToDomain(ExecuteCommandRequest request);
}
