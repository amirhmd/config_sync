using System.Collections.Generic;

namespace ConfigSync.Domain;

public sealed record Page<T>(IReadOnlyList<T> Items, string? NextCursor);
