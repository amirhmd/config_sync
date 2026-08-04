namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record GetDevicesResponse
{
    public DevicePageDetails? Page { get; }
    public ErrorDetails? ErrorDetails { get; }

    private GetDevicesResponse(DevicePageDetails? page, ErrorDetails? errorDetails)
    {
        Page = page;
        ErrorDetails = errorDetails;
    }

    public static GetDevicesResponse Success(DevicePageDetails page) =>
        new(page, null);

    public static GetDevicesResponse Invalid(ErrorDetails errorDetails) =>
        new(null, errorDetails);
}
