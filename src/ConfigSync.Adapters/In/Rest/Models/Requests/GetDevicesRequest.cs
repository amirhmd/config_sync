namespace ConfigSync.Adapters.In.Rest.Models.Requests;

public sealed record GetDevicesRequest(string? Cursor, int? Limit);
