using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DragaliaAPI.Services.Health;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IDistributedCache cache;

    public RedisHealthCheck(IDistributedCache cache)
    {
        this.cache = cache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await this.cache.SetStringAsync("health", "healthy", cancellationToken);
            await this.cache.RemoveAsync("health", cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                status: context.Registration.FailureStatus,
                exception: ex,
                description: "Failed to connect to Redis"
            );
        }
    }
}
