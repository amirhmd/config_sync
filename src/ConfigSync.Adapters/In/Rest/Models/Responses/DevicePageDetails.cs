using System.Collections.Generic;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record DevicePageDetails(IReadOnlyList<DeviceDetails> Items, string? NextCursor);
