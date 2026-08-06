using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfigSync.Adapters.In.Rest;

public static class RestEndpointMappings
{
    public static WebApplication MapAdapterEndpoints(this WebApplication app)
    {
        app.MapPost(Uris.Executions, ExecuteCommand);
        return app;
    }

    private static async Task<IResult> ExecuteCommand(
        [FromBody] ExecuteCommandRequest request,
        [FromServices] IExecuteCommandEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        ExecuteCommandResponse response = await endpoint.Execute(request, cancellationToken);

        string location = $"{Uris.Executions}/{response.ReferenceId}";

        return Results.Created(location, response);
    }
}
