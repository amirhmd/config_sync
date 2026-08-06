using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest;
using ConfigSync.Adapters.Tests.Fakes;
using ConfigSync.Application;
using ConfigSync.Application.Ports.In;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest;

internal sealed class DeviceEndpointTestHost : IAsyncDisposable
{
    private readonly WebApplication _application;
    private readonly HttpClient _client;

    private DeviceEndpointTestHost(WebApplication application, HttpClient client)
    {
        _application = application;
        _client = client;
    }

    public static async Task<DeviceEndpointTestHost> StartAsync()
    {
        // prevent test host to pick up machine configuration.
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(AdaptersTestConfiguration.BuildEncryption());
        builder.Services.AddApplication();
        builder.Services.AddAdapters();

        // Explicit replacement
        builder.Services.RemoveAll<IDeviceUseCase>();
        builder.Services.AddSingleton<IDeviceUseCase, InMemoryDeviceUseCase>();

        var application = builder.Build();
        application.MapAdapterEndpoints();

        try
        {
            await application.StartAsync(TestContext.Current.CancellationToken);
        }
        catch
        {
            await application.DisposeAsync();
            throw;
        }

        return new DeviceEndpointTestHost(application, application.GetTestClient());
    }

    public Task<HttpResponseMessage> PostJsonAsync(string uri, string json)
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return _client.PostAsync(uri, content, TestContext.Current.CancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(string uri)
    {
        return _client.GetAsync(uri, TestContext.Current.CancellationToken);
    }

    public Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        return _client.DeleteAsync(uri, TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();

        // Cleanup must not use the test token; it may already be cancelled.
        await _application.StopAsync(CancellationToken.None);
        await _application.DisposeAsync();
    }
}
