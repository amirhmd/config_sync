using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Domain;

namespace ConfigSync.Application.Ports.In;

public interface IExecuteCommandUseCase
{
    Task<ExecutionReference> Execute(ExecutionPlan plan, CancellationToken cancellationToken);
}
