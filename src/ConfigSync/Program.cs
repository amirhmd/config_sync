using ConfigSync.Adapters;
using ConfigSync.Adapters.In.Rest;
using ConfigSync.Application;
using ConfigSync.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
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
                .AddConsoleExporter((_, metricReaderOptions) =>
                {
                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 6000_000;
                }));
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ConfigSync API",
                Version = "v1",
                Description = "Demo API for device persistence and command execution."
            });
        });
        
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddAdapters(builder.Configuration);

        WebApplication app = builder.Build();
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConfigSync API v1");
            options.RoutePrefix = "swagger";
        });

        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("ConfigSync starting up in {Environment} environment", app.Environment.EnvironmentName);

        
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        
        app.MapAdapterEndpoints();

        app.Run();
    }
}