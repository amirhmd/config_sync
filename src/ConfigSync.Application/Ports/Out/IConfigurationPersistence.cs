using System;
using System.Reactive;
using ConfigSync.Domain.Entities;

namespace ConfigSync.Application.Ports;

public interface IConfigurationPersistence
{
    IObservable<Unit> Upsert(ConfigurationEntry entry);
}