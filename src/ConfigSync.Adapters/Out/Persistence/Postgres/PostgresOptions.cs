namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed class PostgresOptions
{
    public const string SectionName = "Postgres";

    public int MaximumPoolSize { get; init; }
    public int MinimumPoolSize { get; init; }
    public int ConnectionTimeoutSeconds { get; init; } = 15;
    public int CommandTimeoutSeconds { get; init; } = 30;
}