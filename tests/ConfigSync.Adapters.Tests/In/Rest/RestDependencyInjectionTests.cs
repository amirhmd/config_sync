using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Validation;
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

    [Fact]
    public void AddAdapters_RegistersIDeviceMapperAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddSingleton(AdaptersTestConfiguration.BuildEncryption());
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IDeviceMapper>();
        var second = provider.GetRequiredService<IDeviceMapper>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddAdapters_RegistersRequestValidationAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<RequestValidation>();
        var second = provider.GetRequiredService<RequestValidation>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddAdapters_RegistersEveryValueValidation()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.NotNull(provider.GetRequiredService<NameValidation>());
        Assert.NotNull(provider.GetRequiredService<HostValidation>());
        Assert.NotNull(provider.GetRequiredService<PortValidation>());
        Assert.NotNull(provider.GetRequiredService<UsernameValidation>());
        Assert.NotNull(provider.GetRequiredService<CredentialValidation>());
        Assert.NotNull(provider.GetRequiredService<LimitValidation>());
    }
}
