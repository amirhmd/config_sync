using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigSync.Tests;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_RespondsWithSafeProblemDetailsCarryingTheTraceId()
    {
        // given
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = HostTestConfiguration.BuildContext(TestContext.Current.CancellationToken);
        context.TraceIdentifier = "trace-123";
        var exception = new InvalidOperationException("connection string is Host=secret");

        // when
        var handled = await handler.TryHandleAsync(context, exception, TestContext.Current.CancellationToken);

        // then
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var body = await HostTestConfiguration.ReadBodyAsync(context);
        Assert.Contains("trace-123", body, StringComparison.Ordinal);
        Assert.DoesNotContain("secret", body, StringComparison.Ordinal);
        Assert.DoesNotContain("InvalidOperationException", body, StringComparison.Ordinal);
        Assert.DoesNotContain(".cs:line", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TryHandleAsync_WritesNothingWhenTheClientAborted()
    {
        // given
        using var aborted = new CancellationTokenSource();
        await aborted.CancelAsync();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = HostTestConfiguration.BuildContext(aborted.Token);

        // when
        var handled = await handler.TryHandleAsync(context, new OperationCanceledException(), TestContext.Current.CancellationToken);

        // then
        Assert.True(handled);
        Assert.Empty(await HostTestConfiguration.ReadBodyAsync(context));
    }

    [Fact]
    public async Task TryHandleAsync_TreatsAServerSideTimeoutAsAFailureNotACancellation()
    {
        // given — the client is still connected, so this cancellation is our own timeout
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = HostTestConfiguration.BuildContext(TestContext.Current.CancellationToken);

        // when
        await handler.TryHandleAsync(context, new OperationCanceledException(), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }
}
