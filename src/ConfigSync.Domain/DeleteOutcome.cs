namespace ConfigSync.Domain;

public sealed record DeleteOutcome
{
    public OperationOutcome Value { get; }

    private DeleteOutcome(OperationOutcome value) => Value = value;

    public static DeleteOutcome Deleted { get; } = new(OperationOutcome.Deleted);
    public static DeleteOutcome NotFound { get; } = new(OperationOutcome.NotFound);
}
