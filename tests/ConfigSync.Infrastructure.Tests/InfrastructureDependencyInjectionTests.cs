using System.Collections.Generic;
using System.IO;
using ConfigSync.Infrastructure.Credentials;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Infrastructure.Tests;

public class InfrastructureDependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_RegistersIDataProtectionProvider()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();

        // when
        services.AddInfrastructure(InfrastructureTestConfiguration.Build());
        var provider = services.BuildServiceProvider();

        // then
        Assert.NotNull(provider.GetRequiredService<IDataProtectionProvider>());
    }

    [Fact]
    public void AddInfrastructure_RegistersIDeviceCredentialEncryptionAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build());
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IDeviceCredentialEncryption>();
        var second = provider.GetRequiredService<IDeviceCredentialEncryption>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddInfrastructure_UsesConfiguredKeyPath()
    {
        // given
        var keyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["DataProtection:KeyPath"] = keyPath
            }));
        var provider = services.BuildServiceProvider();

        // when
        var protector = provider.GetRequiredService<IDataProtectionProvider>().CreateProtector("test");
        _ = protector.Protect("probe");

        // then
        Assert.True(Directory.Exists(keyPath));

        // cleanup
        Directory.Delete(keyPath, recursive: true);
    }

    [Fact]
    public void AddInfrastructure_FallsBackToDefaultKeyPath_WhenNotConfigured()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();

        // when / then
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["DataProtection:KeyPath"] = null
            }));
        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<IDataProtectionProvider>());
    }
}
