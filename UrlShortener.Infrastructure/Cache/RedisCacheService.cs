using StackExchange.Redis;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Cache;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            var value = await redis.GetDatabase().StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        try
        {
            if (expiry.HasValue)
            {
                await redis.GetDatabase().StringSetAsync(key, value, expiry.Value);
            }
            else
            {
                await redis.GetDatabase().StringSetAsync(key, value);
            }
        }
        catch
        {
        }
    }
}
