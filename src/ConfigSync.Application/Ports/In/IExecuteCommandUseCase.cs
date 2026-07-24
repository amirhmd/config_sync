using System;

namespace ConfigSync.Application.Ports.In;

public interface IExecuteCommandUseCase
{
    IObservable<Guid> Execute(string deviceId, string command);
}