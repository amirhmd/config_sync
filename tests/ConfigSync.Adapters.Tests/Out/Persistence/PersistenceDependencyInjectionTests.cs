using ConfigSync.Application;
using ConfigSync.Application.Ports.Out;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence;

// nothing here checks that Infrastructure actually registers NpgsqlDataProtection or
// IDataProtectionProvider, only that Adapters consumes them correctly when present.
public class PersistenceDependencyInjectionTests
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=configsync;Username=configsync;Password=configsync";

    [Fact]
    public void AddAdapters_RegistersIDevicePersistenceAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddSingleton(NpgsqlDataSource.Create(ConnectionString));
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
