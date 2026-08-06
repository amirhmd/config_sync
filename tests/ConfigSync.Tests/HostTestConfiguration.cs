using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigSync.Tests;

internal static class HostTestConfiguration
{
    internal static DefaultHttpContext BuildContext(CancellationToken requestAborted = default)
    {
        return new DefaultHttpContext
        {
            Request = { Path = "/v1/devices" },
            Response = { Body = new MemoryStream() },
            RequestAborted = requestAborted
        };
    }

    internal static async Task<string> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(context.Response.Body);

        return await reader.ReadToEndAsync();
    }
}
