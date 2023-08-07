using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Redis.OM;
using Redis.OM.Contracts;

namespace DragaliaAPI.Photon.StateManager;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IRedisConnectionProvider connectionprovider;

    public RedisHealthCheck(IRedisConnectionProvider connectionProvider)
    {
        connectionprovider = connectionProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            RedisReply reply = await connectionprovider.Connection.ExecuteAsync("PING");
            if (reply.Error)
            {
                return new HealthCheckResult(
                    status: context.Registration.FailureStatus,
                    exception: null,
                    description: "Ping command returned error"
                );
            }

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
