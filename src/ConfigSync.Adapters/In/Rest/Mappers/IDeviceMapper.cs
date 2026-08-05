using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.In.Rest.Mappers;

public interface IDeviceMapper
{
    /// <summary>
    /// Builds a domain <see cref="Device"/> from a validated create request, encrypting the
    /// credential from whichever of password/private key is present. Throws when the request
    /// does not carry exactly one credential; validation is expected to prevent that.
    /// </summary>
    Device ToDevice(CreateDeviceRequest request);

    DeviceDetails ToDetails(Device device);

    DevicePageDetails ToPageDetails(Page<Device> page);
}
