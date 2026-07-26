using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Models;

namespace ConfigSync.Adapters.In.Rest.Api;

public interface IExecuteCommandEndpoint
{
    Task<ExecuteCommandResponse> Execute(ExecuteCommandRequest request);
}