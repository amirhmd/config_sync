using System;

namespace ConfigSync.Adapters.Out.Persistence.Postgres.Exceptions;

/// <summary>
/// A PostgresSQL operation failed for a reason that may succeed on retry. The originating
/// driver exception is preserved as the inner exception. The host maps this to 503.
/// </summary>
public sealed class PostgresTransientException(string message, Exception innerException) : Exception(message, innerException);
