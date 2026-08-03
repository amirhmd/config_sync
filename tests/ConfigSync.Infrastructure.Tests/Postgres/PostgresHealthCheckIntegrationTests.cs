using System.Threading.Tasks;
using ConfigSync.Infrastructure.Postgres;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ConfigSync.Infrastructure.Tests.Postgres;

[Trait("Category", "Integration")]
public sealed class PostgresHealthCheckIntegrationTests
{
    private const string PostgresImage = "docker.io/library/postgres:17.6-alpine3.22";

    [Fact]
    public async Task CheckHealthAsync_WhenPostgresIsAvailable_ReturnsHealthy()
    {
        // given
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var postgres = new PostgreSqlBuilder(PostgresImage).Build();
        await postgres.StartAsync(cancellationToken);
        await using var dataSource = NpgsqlDataSource.Create(postgres.GetConnectionString());
        var healthCheck = new PostgresHealthCheck(dataSource);

        // when
        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(), cancellationToken);

        // then
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }
}
