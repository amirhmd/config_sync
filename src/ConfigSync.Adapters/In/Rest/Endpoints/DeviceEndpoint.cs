using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.In.Rest.Validation;
using ConfigSync.Application.Ports.In;

namespace ConfigSync.Adapters.In.Rest.Endpoints;

public sealed class DeviceEndpoint(IDeviceUseCase useCase, IDeviceMapper mapper, RequestValidation validation) : IDeviceEndpoint
{
    public async Task<CreateDeviceResponse> Create(CreateDeviceRequest request, CancellationToken cancellationToken)
    {
        var errorDetails = validation.Validate(request);

        if (errorDetails is not null)
        {
            return CreateDeviceResponse.Invalid(errorDetails);
        }

        var device = mapper.ToDevice(request);
        var outcome = await useCase.CreateAsync(device, cancellationToken);

        if (mapper.ToCreateOutcome(outcome) == CreateDeviceOutcome.AlreadyExists)
        {
            return CreateDeviceResponse.AlreadyExists();
        }

        return CreateDeviceResponse.Created(mapper.ToDetails(device));
    }

    public async Task<GetDeviceResponse> Get(GetDeviceRequest request, CancellationToken cancellationToken)
    {
        var errorDetails = validation.Validate(request);

        if (errorDetails is not null)
        {
            return GetDeviceResponse.Invalid(errorDetails);
        }

        var device = await useCase.GetByNameAsync(request.Name, cancellationToken);

        if (device is null)
        {
            return GetDeviceResponse.NotFound();
        }

        return GetDeviceResponse.Found(mapper.ToDetails(device));
    }

    public async Task<GetDevicesResponse> GetPage(GetDevicesRequest request, CancellationToken cancellationToken)
    {
        var errorDetails = validation.Validate(request);

        if (errorDetails is not null)
        {
            return GetDevicesResponse.Invalid(errorDetails);
        }

        var outcome = await useCase.GetPageAsync(request.Cursor, request.Limit, cancellationToken);

        if (outcome.CursorIsInvalid)
        {
            return GetDevicesResponse.Invalid(new ErrorDetails([new ValidationError("cursor", "Cursor is invalid")]));
        }

        return GetDevicesResponse.Success(mapper.ToPageDetails(outcome.Page));
    }

    public async Task<DeleteDeviceResponse> Delete(DeleteDeviceRequest request, CancellationToken cancellationToken)
    {
        var errorDetails = validation.Validate(request);

        if (errorDetails is not null)
        {
            return DeleteDeviceResponse.Invalid(errorDetails);
        }

        var outcome = await useCase.DeleteAsync(request.Name, cancellationToken);

        if (mapper.ToDeleteOutcome(outcome) == DeleteDeviceOutcome.NotFound)
        {
            return DeleteDeviceResponse.NotFound();
        }

        return DeleteDeviceResponse.Deleted();
    }
}
