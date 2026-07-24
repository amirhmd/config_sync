using System;
using System.Reactive.Linq;
using ConfigSync.Application.Ports.In;

namespace ConfigSync.Application.Services;

public class ExecuteCommandService : IExecuteCommandUseCase
{
    public IObservable<Guid> Execute(string deviceId, string command)
    {
        var referenceId = Guid.NewGuid();
        return Observable.Return(referenceId);
    }
}