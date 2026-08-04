using System.Collections.Generic;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

public sealed record ErrorDetails(IReadOnlyDictionary<string, IReadOnlyList<string>> Errors);
