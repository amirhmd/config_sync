namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed record CursorPayload(int Version, long AfterId);
