using UrlShortener.Application.Models;

namespace UrlShortener.Application.Abstractions;

public interface IUrlService
{
    Task<ShortUrlResult> CreateShortUrl(
        int userId,
        string originalUrl,
        string? customAlias,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default);

    Task<PagedResult<UserUrlResult>> GetUserUrls(int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<UrlRedirectResult?> GetOriginalUrl(
        string shortCode,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<UrlAnalyticsResult?> GetUrlAnalytics(int userId, int urlId, CancellationToken cancellationToken = default);

    Task DeleteUrl(int userId, int urlId, CancellationToken cancellationToken = default);
}
