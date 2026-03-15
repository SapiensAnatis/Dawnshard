using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DragaliaAPI.Infrastructure;

public partial class RedisHealthCheck : IHealthCheck
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
            Log.RedisHealthCheckFailed(this.logger, ex);

            return new(
                status: context.Registration.FailureStatus,
                exception: ex,
                description: "Failed to connect to Redis"
            );
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Redis health check failed")]
        public static partial void RedisHealthCheckFailed(ILogger logger, Exception exception);
    }
}
