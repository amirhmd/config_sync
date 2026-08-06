using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Endpoints;

public class ExecuteCommandEndpointTests
{
    [Fact]
    public async Task Execute_UsesRealMapperAndRealUseCase_ReturnsNonEmptyReferenceId()
    {
        // given
        IMeterFactory meterFactory = new ServiceCollection()
            .AddMetrics()
            .BuildServiceProvider()
            .GetRequiredService<IMeterFactory>();
        IExecutionPlanMapper mapper = new ExecutionPlanMapper();
        IExecuteCommandUseCase useCase = new ExecuteCommandService(
            NullLogger<ExecuteCommandService>.Instance, meterFactory);
        ExecuteCommandEndpoint endpoint = new ExecuteCommandEndpoint(useCase, mapper);
        ExecuteCommandRequest request = new ExecuteCommandRequest
        {
            Commands = ["show version", "show interfaces"],
            DeviceIds = ["device1", "device2"]
        };

        // when
        ExecuteCommandResponse response =
            await endpoint.Execute(request, TestContext.Current.CancellationToken);

        // then
        Assert.NotEqual(Guid.Empty, response.ReferenceId);
    }
}
