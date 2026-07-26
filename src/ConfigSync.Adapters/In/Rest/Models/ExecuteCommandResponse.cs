using System;

namespace ConfigSync.Adapters.In.Rest.Models;

public class ExecuteCommandResponse
{
    public required Guid ReferenceId { get; init; }
}