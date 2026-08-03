using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConfigSync.Infrastructure.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace ConfigSync.Infrastructure.Tests.Health;

internal sealed class HealthEndpointTestHost : IAsyncDisposable
{
    private const string ReadinessCheckName = "postgres";
    private const string ReadinessTag = "readiness";

    private readonly WebApplication _application;
    private readonly HttpClient _client;

    private HealthEndpointTestHost(WebApplication application, HttpClient client)
    {
        _application = application;
        _client = client;
    }

    public static async Task<HealthEndpointTestHost> StartAsync(
        HealthCheckResult? readinessResult = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        var healthChecks = builder.Services.AddHealthChecks();

        if (readinessResult is { } result)
        {
            healthChecks.AddCheck(ReadinessCheckName, () => result, tags: [ReadinessTag]);
        }

        var application = builder.Build();
        application.MapHealthEndpoints();
        await application.StartAsync(TestContext.Current.CancellationToken);
        var client = application.GetTestClient();
        return new HealthEndpointTestHost(application, client);
    }

    public Task<HttpResponseMessage> GetAsync(string uri)
    {
        return _client.GetAsync(uri, TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _application.StopAsync(TestContext.Current.CancellationToken);
        await _application.DisposeAsync();
    }
}
