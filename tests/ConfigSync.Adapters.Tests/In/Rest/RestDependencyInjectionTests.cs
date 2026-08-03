using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Application;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest;

public class RestDependencyInjectionTests
{
    [Fact]
    public void AddAdapters_RegistersIExecutionPlanMapperAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IExecutionPlanMapper>();
        var second = provider.GetRequiredService<IExecutionPlanMapper>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddAdapters_RegistersIExecuteCommandEndpointAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IExecuteCommandEndpoint>();
        var second = provider.GetRequiredService<IExecuteCommandEndpoint>();

        // then
        Assert.Same(first, second);
    }
}
