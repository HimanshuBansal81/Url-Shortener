using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Cache;

public class NullCacheService : ICacheService
{
    public Task<string?> GetAsync(string key) => Task.FromResult<string?>(null);

    public Task SetAsync(string key, string value, TimeSpan? expiry = null) => Task.CompletedTask;
}
