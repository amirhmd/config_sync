using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models;
using ConfigSync.Application.Ports.In;

namespace ConfigSync.Adapters.In.Rest.Endpoints;

public class ExecuteCommandEndpoint(IExecuteCommandUseCase useCase, IExecutionPlanMapper mapper)
    : IExecuteCommandEndpoint
{
    public async Task<ExecuteCommandResponse> Execute(ExecuteCommandRequest request)
    {
        var plan = mapper.ToDomain(request);
        var executionReference = await useCase.Execute(plan);
        return new ExecuteCommandResponse { ReferenceId = executionReference.ReferenceId };
    }
}