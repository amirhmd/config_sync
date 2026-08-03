using System;
using System.Collections.Generic;
using ConfigSync.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Xunit;

namespace ConfigSync.Infrastructure.Tests.Postgres;

public class PostgresDependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_RegistersNpgsqlDataSourceAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build());
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<NpgsqlDataSource>();
        var second = provider.GetRequiredService<NpgsqlDataSource>();

        // then
        Assert.Same(first, second);
    }

    [Fact]
    public void AddInfrastructure_MissingConnectionString_ThrowsAtRegistration()
    {
        // given
        var configuration = InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = null
            });
        var services = new ServiceCollection();
        services.AddLogging();

        // when / then
        Assert.Throws<InvalidOperationException>(() =>
            services.AddInfrastructure(configuration));
    }

    [Fact]
    public void AddInfrastructure_InvalidMaximumPoolSize_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:MaximumPoolSize"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddInfrastructure_NegativeMinimumPoolSize_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:MinimumPoolSize"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddInfrastructure_MinimumExceedsMaximum_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:MinimumPoolSize"] = "30",
                ["Postgres:MaximumPoolSize"] = "20"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddInfrastructure_InvalidConnectionTimeoutSeconds_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:ConnectionTimeoutSeconds"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }

    [Fact]
    public void AddInfrastructure_InvalidCommandTimeoutSeconds_ThrowsOnResolution()
    {
        // given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(InfrastructureTestConfiguration.Build(new Dictionary<string, string?>
            {
                ["Postgres:CommandTimeoutSeconds"] = "-1"
            }));
        var provider = services.BuildServiceProvider();

        // when / then
        Assert.Throws<OptionsValidationException>(provider.GetRequiredService<NpgsqlDataSource>);
    }
}
