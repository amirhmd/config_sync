using System;
using ConfigSync.Domain;

namespace ConfigSync.Application.Ports.In;

public interface IExecuteCommandUseCase
{
    IObservable<Guid> Execute(ExecutionPlan plan);
}