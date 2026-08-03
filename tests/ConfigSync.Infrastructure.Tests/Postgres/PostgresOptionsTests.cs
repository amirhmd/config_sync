using ConfigSync.Infrastructure.Postgres;
using Xunit;

namespace ConfigSync.Infrastructure.Tests.Postgres;

public class PostgresOptionsTests
{
    [Fact]
    public void Constructor_UsesExpectedDefaults()
    {
        // given
        var options = new PostgresOptions();

        // when then
        Assert.Equal(0, options.MaximumPoolSize);
        Assert.Equal(0, options.MinimumPoolSize);
        Assert.Equal(15, options.ConnectionTimeoutSeconds);
        Assert.Equal(30, options.CommandTimeoutSeconds);
    }

    [Fact]
    public void Initializer_AssignsConfiguredValues()
    {
        // given
        var options = new PostgresOptions
        {
            MaximumPoolSize = 100,
            MinimumPoolSize = 10,
            ConnectionTimeoutSeconds = 5,
            CommandTimeoutSeconds = 60
        };

        // when then
        Assert.Equal(100, options.MaximumPoolSize);
        Assert.Equal(10, options.MinimumPoolSize);
        Assert.Equal(5, options.ConnectionTimeoutSeconds);
        Assert.Equal(60, options.CommandTimeoutSeconds);
    }

    [Fact]
    public void SectionName_HasExpectedValue()
    {
        // given when then
        Assert.Equal("Postgres", PostgresOptions.SectionName);
    }
}