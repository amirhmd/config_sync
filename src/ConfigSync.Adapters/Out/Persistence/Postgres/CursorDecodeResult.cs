namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed class CursorDecodeResult
{
    public bool IsValid { get; }
    public long AfterId { get; }

    private CursorDecodeResult(bool isValid, long afterId)
    {
        IsValid = isValid;
        AfterId = afterId;
    }

    public static CursorDecodeResult Success(long afterId) => new(true, afterId);

    public static CursorDecodeResult Invalid() => new(false, 0);
}
