using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Postgres;

public class PostgresDependencyInjectionTests
{
    [Fact]
    public void AddAdapters_RegistersNpgsqlDataSourceAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdapters(AdaptersTestConfiguration.Build());
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<NpgsqlDataSource>();
        var second = provider.GetRequiredService<NpgsqlDataSource>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddAdapters_MissingConnectionString_ThrowsAtRegistration()
    {
        // given
        var configuration = AdaptersTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = null
            });
        var services = new ServiceCollection();
        services.AddLogging();

        // when / then
        Assert.Throws<InvalidOperationException>(() => services.AddAdapters(configuration));
    }

    [Fact]
    public void AddAdapters_InvalidMaximumPoolSize_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdapters(AdaptersTestConfiguration.Build(
            new Dictionary<string, string?>
            {
                ["Postgres:MaximumPoolSize"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddAdapters_NegativeMinimumPoolSize_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdapters(AdaptersTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:MinimumPoolSize"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddAdapters_MinimumExceedsMaximum_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdapters(AdaptersTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:MinimumPoolSize"] = "30",
                ["Postgres:MaximumPoolSize"] = "20"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }
}