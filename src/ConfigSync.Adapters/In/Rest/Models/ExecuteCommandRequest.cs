using System.Collections.Generic;

namespace ConfigSync.Adapters.In.Rest.Models;

public class ExecuteCommandRequest
{
    public List<string> Commands { get; init; } = [];
    public List<string> DeviceIds { get; init; } = [];
}