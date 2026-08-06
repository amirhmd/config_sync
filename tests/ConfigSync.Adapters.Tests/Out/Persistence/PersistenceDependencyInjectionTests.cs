using ConfigSync.Application;
using ConfigSync.Application.Ports.Out;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence;

public class PersistenceDependencyInjectionTests
{
    [Fact]
    public void AddAdapters_RegistersIDevicePersistenceAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddAdapters();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IDevicePersistence>();
        var second = provider.GetRequiredService<IDevicePersistence>();

        // then
        Assert.Same(first, second);
    }
}
