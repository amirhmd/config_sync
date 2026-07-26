using System.Collections.Immutable;

namespace ConfigSync.Domain;

public sealed record ExecutionPlan(
    ImmutableList<string> Commands,
    ImmutableList<string> DeviceIds);