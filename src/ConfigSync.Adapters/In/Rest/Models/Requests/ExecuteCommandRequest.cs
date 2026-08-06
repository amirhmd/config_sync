using System.Collections.Generic;

namespace ConfigSync.Adapters.In.Rest.Models.Requests;

public class ExecuteCommandRequest
{
    public List<string> Commands { get; init; } = [];
    public List<string> DeviceIds { get; init; } = [];
}
