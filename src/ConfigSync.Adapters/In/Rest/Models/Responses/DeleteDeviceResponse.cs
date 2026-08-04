namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record DeleteDeviceResponse
{
    public DeleteDeviceOutcome? Outcome { get; }
    public ErrorDetails? ErrorDetails { get; }

    private DeleteDeviceResponse(
        DeleteDeviceOutcome? outcome,
        ErrorDetails? errorDetails)
    {
        Outcome = outcome;
        ErrorDetails = errorDetails;
    }

    public static DeleteDeviceResponse Deleted() =>
        new(DeleteDeviceOutcome.Deleted, null);

    public static DeleteDeviceResponse NotFound() =>
        new(DeleteDeviceOutcome.NotFound, null);

    public static DeleteDeviceResponse Invalid(ErrorDetails errorDetails) =>
        new(null, errorDetails);
}
