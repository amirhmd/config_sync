using System;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Domain;
using Microsoft.Extensions.Logging;

namespace ConfigSync.Application.Services;

public class ExecuteCommandService(ILogger<ExecuteCommandService> logger)
    : IExecuteCommandUseCase
{
    public Task<ExecutionReference> Execute(ExecutionPlan plan, CancellationToken cancellationToken)
    {
        var referenceId = Guid.NewGuid();

        logger.LogInformation(
            "Executing plan {ReferenceId} for {DeviceCount} devices with {CommandCount} commands",
            referenceId, plan.DeviceIds.Count, plan.Commands.Count);

        return Task.FromResult(new ExecutionReference(referenceId));
    }
}
