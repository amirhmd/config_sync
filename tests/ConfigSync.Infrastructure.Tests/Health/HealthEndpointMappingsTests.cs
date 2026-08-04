using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace ConfigSync.Infrastructure.Tests.Health;

[Trait("Category", "Integration")]
public sealed class HealthEndpointMappingsTests
{
    [Fact]
    public async Task LivenessEndpoint_ReturnsOk_WhenNoChecksAreRegistered()
    {
        // given
        await using var host = await HealthEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync("/health/live");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadinessEndpoint_ReturnsOk_WhenNoReadinessChecksAreRegistered()
    {
        // given
        await using var host = await HealthEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync("/health/ready");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LivenessEndpoint_ReturnsOk_WhenReadinessCheckIsUnhealthy()
    {
        // given
        await using var host = await HealthEndpointTestHost.StartAsync(
            HealthCheckResult.Unhealthy("PostgresSQL unavailable."));

        // when
        using var response = await host.GetAsync("/health/live");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadinessEndpoint_ReturnsServiceUnavailable_WhenReadinessCheckIsUnhealthy()
    {
        // given
        await using var host = await HealthEndpointTestHost.StartAsync(
            HealthCheckResult.Unhealthy("PostgresSQL unavailable."));

        // when
        using var response = await host.GetAsync("/health/ready");

        // then
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
