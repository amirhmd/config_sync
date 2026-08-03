using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ConfigSync.Infrastructure.Health;

public static class HealthEndpointMappings
{
    private const string LivenessUri = "/health/live";
    private const string ReadinessUri = "/health/ready";
    private const string ReadinessTag = "readiness";

    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        app.MapLivenessEndpoint();
        app.MapReadinessEndpoint();
        return app;
    }

    private static void MapLivenessEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(LivenessUri, new HealthCheckOptions
        {
            Predicate = _ => false
        });
    }

    private static void MapReadinessEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(ReadinessUri, new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(ReadinessTag)
        });
    }
}
