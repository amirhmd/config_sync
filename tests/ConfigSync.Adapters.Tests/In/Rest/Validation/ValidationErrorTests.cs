using System;
using ConfigSync.Adapters.In.Rest.Validation;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Validation;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_SetsPropertyNameAndMessage()
    {
        // given / when
        var error = new ValidationError("name", "Name is required.");

        // then
        Assert.Equal("name", error.PropertyName);
        Assert.Equal("Name is required.", error.Message);
    }

    [Fact]
    public void Constructor_ThrowsWhenPropertyNameIsBlank()
    {
        // given / when / then
        Assert.Throws<ArgumentException>(() => new ValidationError("   ", "Message."));
    }

    [Fact]
    public void Constructor_ThrowsWhenMessageIsBlank()
    {
        // given / when / then
        Assert.Throws<ArgumentException>(() => new ValidationError("name", "   "));
    }
}
