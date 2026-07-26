using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ConfigSync.Adapters.In.Rest;

public static class RestEndpointMappings
{
    public static WebApplication MapAdapterEndpoints(this WebApplication app)
    {
        app.MapPost(Uris.Executions, async (
            ExecuteCommandRequest request,
            IExecuteCommandEndpoint endpoint) =>
        {
            ExecuteCommandResponse response = await endpoint.Execute(request);
            return Results.Created($"{Uris.Executions}/{response.ReferenceId}", response);
        });

        return app;
    }
}