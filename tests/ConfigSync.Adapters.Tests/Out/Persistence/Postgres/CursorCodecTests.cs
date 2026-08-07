using System;
using ConfigSync.Adapters.Out.Persistence.Postgres;
using Microsoft.AspNetCore.DataProtection;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Postgres;

public class CursorCodecTests
{
    private const string ProtectorPurpose = "ConfigSync.DeviceCursor";

    [Fact]
    public void EncodeAndDecode_RoundTripTheAfterId()
    {
        // given
        var provider = new EphemeralDataProtectionProvider();
        var codec = new CursorCodec(provider);

        // when
        var cursor = codec.Encode(42);
        var result = codec.Decode(cursor);

        // then
        Assert.True(result.IsValid);
        Assert.Equal(42, result.AfterId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Encode_RejectsNonPositiveAfterId(long afterId)
    {
        // given
        var codec = new CursorCodec(new EphemeralDataProtectionProvider());

        // when / then
        Assert.Throws<ArgumentOutOfRangeException>(() => codec.Encode(afterId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Decode_ReturnsInvalidForBlankCursor(string cursor)
    {
        // given
        var codec = new CursorCodec(new EphemeralDataProtectionProvider());

        // when
        var result = codec.Decode(cursor);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Decode_ReturnsInvalidWhenCursorCannotBeUnprotected()
    {
        // given
        var firstCodec = new CursorCodec(new EphemeralDataProtectionProvider());
        var secondCodec = new CursorCodec(new EphemeralDataProtectionProvider());
        var cursor = firstCodec.Encode(42);

        // when
        var result = secondCodec.Decode(cursor);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Decode_ReturnsInvalidForMalformedPayload()
    {
        // given
        var provider = new EphemeralDataProtectionProvider();
        var codec = new CursorCodec(provider);

        var cursor = provider.CreateProtector(ProtectorPurpose).Protect("not-json");

        // when
        var result = codec.Decode(cursor);

        // then
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Decode_ReturnsInvalidForUnsupportedVersion()
    {
        // given
        var provider = new EphemeralDataProtectionProvider();
        var codec = new CursorCodec(provider);

        var cursor = provider.CreateProtector(ProtectorPurpose).Protect("""{"Version":2,"AfterId":42}""");

        // when
        var result = codec.Decode(cursor);

        // then
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Decode_ReturnsInvalidForNonPositiveAfterId(long afterId)
    {
        // given
        var provider = new EphemeralDataProtectionProvider();
        var codec = new CursorCodec(provider);

        var cursor = provider.CreateProtector(ProtectorPurpose).Protect($$"""{"Version":1,"AfterId":{{afterId}}}""");

        // when
        var result = codec.Decode(cursor);

        // then
        Assert.False(result.IsValid);
    }
}