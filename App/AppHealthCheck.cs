using Microsoft.Extensions.Diagnostics.HealthChecks;
using VerticalSlice.Infra;

namespace VerticalSlice;

public class AppHealthCheck(RedisConnection redisConnection) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(redisConnection.IsConnected() ? HealthCheckResult.Healthy("OK") : HealthCheckResult.Unhealthy("Redis not connected"));
    }
}