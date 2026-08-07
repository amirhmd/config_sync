using System;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.Out.Persistence.Postgres.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConfigSync;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (ClientDisconnected(httpContext, exception))
        {
            logger.LogWarning(
                "Request {Method} {Path} was cancelled by the client. Trace {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);

            return true;
        }

        if (exception is PostgresTransientException)
        {
            logger.LogWarning(
                exception,
                "A dependency reported a transient failure on {Method} {Path}. Trace {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);

            await WriteProblemAsync(
                httpContext,
                StatusCodes.Status503ServiceUnavailable,
                "The service is temporarily unavailable. Please retry.",
                cancellationToken);

            return true;
        }

        logger.LogError(
            exception,
            "Unhandled exception on {Method} {Path}. Trace {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier);

        await WriteProblemAsync(
            httpContext, 
            StatusCodes.Status500InternalServerError, 
            "An unexpected error occurred.", 
            cancellationToken);

        return true;
    }

    // A server-side timeout also throws OperationCanceledException, so the exception type
    // alone cannot be trusted. Only an aborted request means the client is gone.
    private static bool ClientDisconnected(HttpContext httpContext, Exception exception)
    {
        return exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested;
    }

    private static async Task WriteProblemAsync(HttpContext httpContext, int statusCode, string title, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
