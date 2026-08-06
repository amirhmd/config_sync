using ConfigSync.Adapters;
using ConfigSync.Adapters.In.Rest;
using ConfigSync.Application;
using ConfigSync.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using ConfigSync.Infrastructure.Health;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
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
        
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // Infrastructure first: registers NpgsqlDataSource, IDeviceCredentialEncryption, and Data Protection
        // Adapters resolve them later from the container.
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddAdapters();

        WebApplication app = builder.Build();
        
        app.UseExceptionHandler();
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
        app.MapHealthEndpoints();

        app.Run();
    }
}