using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConfigSync.Adapters.Tests;

internal static class AdaptersTestConfiguration
{
    internal static IConfiguration Build(
        Dictionary<string, string?>? overrides = null)
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] =
                "Host=localhost;Port=5432;Database=configsync;Username=configsync;Password=configsync",
            ["Postgres:MaximumPoolSize"] = "20",
            ["Postgres:MinimumPoolSize"] = "0",
            ["Postgres:ConnectionTimeoutSeconds"] = "15",
            ["Postgres:CommandTimeoutSeconds"] = "30"
        };

        if (overrides is not null)
            foreach (var (key, value) in overrides)
                values[key] = value;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}