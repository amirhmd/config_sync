using System;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public class ExecuteCommandResponse
{
    public required Guid ReferenceId { get; init; }
}
