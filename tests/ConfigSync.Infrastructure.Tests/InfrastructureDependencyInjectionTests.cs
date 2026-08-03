using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Infrastructure.Tests;

public class InfrastructureDependencyInjectionTests
{
    private static IConfiguration BuildConfiguration(string? keyPath = null)
    {
        var values = new System.Collections.Generic.Dictionary<string, string?>();

        if (keyPath is not null)
            values["DataProtection:KeyPath"] = keyPath;

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Fact]
    public void AddInfrastructure_RegistersIDataProtectionProvider()
    {
        // given
        var keyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var services = new ServiceCollection();

        // when
        services.AddInfrastructure(BuildConfiguration(keyPath));
        var provider = services.BuildServiceProvider();

        // then
        var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
        Assert.NotNull(dataProtectionProvider);

        // cleanup
        if (Directory.Exists(keyPath))
            Directory.Delete(keyPath, recursive: true);
    }

    [Fact]
    public void AddInfrastructure_UsesConfiguredKeyPath()
    {
        // given
        var keyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var services = new ServiceCollection();
        services.AddInfrastructure(BuildConfiguration(keyPath));
        var provider = services.BuildServiceProvider();

        // when — forces key ring initialization
        var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
        var protector = dataProtectionProvider.CreateProtector("test");
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

        // when / then — should not throw when no KeyPath is configured
        services.AddInfrastructure(BuildConfiguration());
        var provider = services.BuildServiceProvider();
        var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
        Assert.NotNull(dataProtectionProvider);
    }
}