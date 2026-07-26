using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace ConfigSync;

[UsedImplicitly]
public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((_, config) =>
            config.WriteTo.Console());

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: "ConfigSync"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddMeter("ConfigSync.Application")
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

        WebApplication app = builder.Build();

        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("ConfigSync starting up in {Environment} environment", app.Environment.EnvironmentName);

        app.Run();
    }
}