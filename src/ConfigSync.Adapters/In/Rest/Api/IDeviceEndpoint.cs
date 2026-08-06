using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;

namespace ConfigSync.Adapters.In.Rest.Api;

public interface IDeviceEndpoint
{
    Task<CreateDeviceResponse> Create(CreateDeviceRequest request, CancellationToken cancellationToken);

    Task<GetDeviceResponse> Get(GetDeviceRequest request, CancellationToken cancellationToken);

    Task<GetDevicesResponse> GetPage(GetDevicesRequest request, CancellationToken cancellationToken);

    Task<DeleteDeviceResponse> Delete(DeleteDeviceRequest request, CancellationToken cancellationToken);
}
