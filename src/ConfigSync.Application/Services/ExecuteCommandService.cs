using System;
using System.Diagnostics.Metrics;
using System.Reactive.Linq;
using ConfigSync.Application.Ports.In;
using ConfigSync.Domain;
using Microsoft.Extensions.Logging;

namespace ConfigSync.Application.Services;

public class ExecuteCommandService(ILogger<ExecuteCommandService> logger, IMeterFactory meterFactory)
    : IExecuteCommandUseCase
{
    private readonly Counter<int> _executionPlansSubmitted =
        meterFactory
            .Create("ConfigSync.Application")
            .CreateCounter<int>("configsync.execution_plans_submitted");

    public IObservable<Guid> Execute(ExecutionPlan plan)
    {
        var referenceId = Guid.NewGuid();

        logger.LogInformation(
            "Executing plan {ReferenceId} for {DeviceCount} devices with {CommandCount} commands",
            referenceId, plan.DeviceIds.Count, plan.Commands.Count);

        _executionPlansSubmitted.Add(1);

        return Observable.Return(referenceId);
    }
}