using System.Collections.Generic;
using System.Linq;
using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
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
        services.AddSingleton(AdaptersTestFixtures.BuildEncryption());
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
    public void AddAdapters_RegistersEveryCreateDeviceValidation()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var validations = provider
            .GetRequiredService<IEnumerable<IValidation<CreateDeviceRequest>>>()
            .ToList();

        // then
        Assert.Contains(validations, validation => validation is DeviceNameValidation);
        Assert.Contains(validations, validation => validation is DeviceHostValidation);
        Assert.Contains(validations, validation => validation is DevicePortValidation);
        Assert.Contains(validations, validation => validation is DeviceUsernameValidation);
        Assert.Contains(validations, validation => validation is DeviceCredentialValidation);
    }

    [Fact]
    public void AddAdapters_RegistersPageLimitValidation()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var validations = provider
            .GetRequiredService<IEnumerable<IValidation<GetDevicesRequest>>>()
            .ToList();

        // then
        Assert.Contains(validations, validation => validation is PageLimitValidation);
    }
}
