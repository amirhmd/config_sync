namespace ConfigSync.Domain;

public sealed record CreateOutcome
{
    public OperationOutcome Value { get; }

    private CreateOutcome(OperationOutcome value) => Value = value;

    public static CreateOutcome Created { get; } = new(OperationOutcome.Created);
    public static CreateOutcome AlreadyExists { get; } = new(OperationOutcome.AlreadyExists);
}
