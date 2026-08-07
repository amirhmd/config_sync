using System;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed class CursorCodec(IDataProtectionProvider dataProtectionProvider)
{
    private const string ProtectorPurpose = "ConfigSync.DeviceCursor";
    private const int SupportedVersion = 1;
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);

    public string Encode(long afterId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(afterId);

        var payload = new CursorPayload(SupportedVersion, afterId);

        return _protector.Protect(JsonSerializer.Serialize(payload));
    }

    public CursorDecodeResult Decode(string cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return CursorDecodeResult.Invalid();
        }

        CursorPayload? payload;

        try
        {
            payload = JsonSerializer.Deserialize<CursorPayload>(_protector.Unprotect(cursor));
        }
        catch (CryptographicException)
        {
            return CursorDecodeResult.Invalid();
        }
        catch (JsonException)
        {
            return CursorDecodeResult.Invalid();
        }

        // the payload is meaningful for pagination?
        if (payload is null || payload.Version != SupportedVersion || payload.AfterId <= 0)
        {
            return CursorDecodeResult.Invalid();
        }

        return CursorDecodeResult.Success(payload.AfterId);
    }
}
