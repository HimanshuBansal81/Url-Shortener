using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace UrlShortener.Infrastructure.Health;

public class RedisHealthCheck(IConnectionMultiplexer? redis = null) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (redis is null)
        {
            return HealthCheckResult.Healthy("Redis is not configured.");
        }

        try
        {
            var database = redis.GetDatabase();
            await database.PingAsync();
            return HealthCheckResult.Healthy("Redis connection is healthy.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed.", exception);
        }
    }
}
