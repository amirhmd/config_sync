using System;

namespace ConfigSync.Domain.Entities;

/**
 * Immutable
 * inheritance not allowed
 * plain data holder
 */
public sealed record ConfigurationEntry(
    Guid Id,
    string Key,
    string Value,
    string Source,
    DateTimeOffset FetchedAt);
