using StackExchange.Redis;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Cache;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        if (expiry.HasValue)
        {
            await _db.StringSetAsync(key, value, expiry.Value);
        }
        else
        {
            await _db.StringSetAsync(key, value);
        }
    }
}