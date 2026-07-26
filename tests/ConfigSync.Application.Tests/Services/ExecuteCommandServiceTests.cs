using System;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Services;
using ConfigSync.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigSync.Application.Tests.Services;

public class ExecuteCommandServiceTests
{
    [Fact]
    public async Task Execute_ReturnsANonEmptyReferenceId()
    {
        // given
        var meterFactory = new ServiceCollection()
            .AddMetrics()
            .BuildServiceProvider()
            .GetRequiredService<IMeterFactory>();
        IExecuteCommandUseCase given = new ExecuteCommandService(
            NullLogger<ExecuteCommandService>.Instance, meterFactory);
        var plan = new ExecutionPlan(
            Commands: ImmutableList.Create("show version"),
            DeviceIds: ImmutableList.Create("device123"));

        // when
        Guid referenceId = await given.Execute(plan).ToTask();

        // then
        Assert.NotEqual(Guid.Empty, referenceId);
    }
}