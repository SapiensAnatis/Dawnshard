using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DragaliaAPI.Services.Health;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IDistributedCache cache;
    private readonly ILogger<RedisHealthCheck> logger;

    public RedisHealthCheck(IDistributedCache cache, ILogger<RedisHealthCheck> logger)
    {
        this.cache = cache;
        this.logger = logger;
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
            this.logger.LogError(ex, "Redis health check failed");

            return new HealthCheckResult(
                status: context.Registration.FailureStatus,
                exception: ex,
                description: "Failed to connect to Redis"
            );
        }
    }
}
