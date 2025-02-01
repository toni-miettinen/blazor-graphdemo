using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VerticalSlice;

public class AppHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy("OK"));
    }
}