using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;

namespace ConfigSync.Adapters.In.Rest.Api;

public interface IExecuteCommandEndpoint
{
    Task<ExecuteCommandResponse> Execute(
        ExecuteCommandRequest request,
        CancellationToken cancellationToken);
}
