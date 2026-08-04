namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record GetDeviceResponse
{
    public DeviceDetails? Device { get; }
    public ErrorDetails? ErrorDetails { get; }

    private GetDeviceResponse(DeviceDetails? device, ErrorDetails? errorDetails)
    {
        Device = device;
        ErrorDetails = errorDetails;
    }

    public static GetDeviceResponse Found(DeviceDetails device) =>
        new(device, null);

    public static GetDeviceResponse NotFound() =>
        new(null, null);

    public static GetDeviceResponse Invalid(ErrorDetails errorDetails) =>
        new(null, errorDetails);
}
