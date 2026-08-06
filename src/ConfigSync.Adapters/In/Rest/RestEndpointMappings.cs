using System;
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

        var devices = app.MapGroup(Uris.Devices).WithTags("Devices");

        devices.MapPost("", CreateDevice)
            .WithName("CreateDevice")
            .Produces<CreateDeviceResponse>(StatusCodes.Status201Created)
            .Produces<CreateDeviceResponse>(StatusCodes.Status400BadRequest)
            .Produces<CreateDeviceResponse>(StatusCodes.Status409Conflict);

        devices.MapGet("", GetDevices)
            .WithName("GetDevices")
            .Produces<GetDevicesResponse>()
            .Produces<GetDevicesResponse>(StatusCodes.Status400BadRequest);

        devices.MapGet("/{name}", GetDevice)
            .WithName("GetDevice")
            .Produces<GetDeviceResponse>()
            .Produces<GetDeviceResponse>(StatusCodes.Status400BadRequest)
            .Produces<GetDeviceResponse>(StatusCodes.Status404NotFound);

        devices.MapDelete("/{name}", DeleteDevice)
            .WithName("DeleteDevice")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<DeleteDeviceResponse>(StatusCodes.Status400BadRequest)
            .Produces<DeleteDeviceResponse>(StatusCodes.Status404NotFound);

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

    private static async Task<IResult> CreateDevice(
        [FromBody] CreateDeviceRequest request,
        [FromServices] IDeviceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        CreateDeviceResponse response = await endpoint.Create(request, cancellationToken);

        if (response.ErrorDetails is not null)
        {
            return Results.BadRequest(response);
        }

        // Only the Created factory carries a device; AlreadyExists leaves it absent.
        if (response.Device is null)
        {
            return Results.Conflict(response);
        }

        string location = $"{Uris.Devices}/{Uri.EscapeDataString(response.Device.Name)}";

        return Results.Created(location, response);
    }

    private static async Task<IResult> GetDevice(
        [FromRoute] string name,
        [FromServices] IDeviceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        var request = new GetDeviceRequest(name);

        GetDeviceResponse response = await endpoint.Get(request, cancellationToken);

        if (response.ErrorDetails is not null)
        {
            return Results.BadRequest(response);
        }

        if (response.Device is null)
        {
            return Results.NotFound(response);
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> GetDevices(
        [AsParameters] GetDevicesRequest request,
        [FromServices] IDeviceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        GetDevicesResponse response = await endpoint.GetPage(request, cancellationToken);

        if (response.ErrorDetails is not null)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteDevice(
        [FromRoute] string name,
        [FromServices] IDeviceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        var request = new DeleteDeviceRequest(name);

        DeleteDeviceResponse response = await endpoint.Delete(request, cancellationToken);

        if (response.ErrorDetails is not null)
        {
            return Results.BadRequest(response);
        }

        if (response.Outcome == DeleteDeviceOutcome.NotFound)
        {
            return Results.NotFound(response);
        }

        return Results.NoContent();
    }
}
