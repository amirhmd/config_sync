namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record CreateDeviceResponse
{
    public CreateDeviceOutcome? Outcome { get; }
    public DeviceDetails? Device { get; }
    public ErrorDetails? ErrorDetails { get; }

    private CreateDeviceResponse(
        CreateDeviceOutcome? outcome,
        DeviceDetails? device,
        ErrorDetails? errorDetails)
    {
        Outcome = outcome;
        Device = device;
        ErrorDetails = errorDetails;
    }

    public static CreateDeviceResponse Created(DeviceDetails device) =>
        new(CreateDeviceOutcome.Created, device, null);

    public static CreateDeviceResponse AlreadyExists() =>
        new(CreateDeviceOutcome.AlreadyExists, null, null);

    public static CreateDeviceResponse Invalid(ErrorDetails errorDetails) =>
        new(null, null, errorDetails);
}
