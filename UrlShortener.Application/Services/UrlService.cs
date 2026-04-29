using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Models;
using UrlShortener.Application.Validators;
using UrlShortener.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Application.Services;

public class UrlService(
    IUrlRepository urlRepository,
    IUserRepository userRepository,
    ICacheService cacheService,
    ILogger<UrlService> logger) : IUrlService
{
    public async Task<ShortUrlResult> CreateShortUrl(
        int userId,
        string originalUrl,
        string? customAlias,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("A valid user is required.", nameof(userId));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var parsedUri = UrlValidator.ValidateOriginalUrl(originalUrl);
        var validatedAlias = CustomAliasValidator.Validate(customAlias);

        if (expiresAt.HasValue && expiresAt.Value <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiry must be a future date.", nameof(expiresAt));
        }

        if (validatedAlias is not null && await urlRepository.ShortCodeExistsAsync(validatedAlias, cancellationToken))
        {
            throw new InvalidOperationException("This custom alias is already in use.");
        }

        var url = new Url
        {
            UserId = userId,
            OriginalUrl = parsedUri.ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            ShortCode = validatedAlias ?? "tmp"
        };

        await urlRepository.AddAsync(url, cancellationToken);
        await urlRepository.SaveChangesAsync(cancellationToken);

        if (validatedAlias is null)
        {
            url.ShortCode = Base62Encoder.Encode(url.Id);
        }

        await urlRepository.SaveChangesAsync(cancellationToken);

        return new ShortUrlResult(
            url.Id,
            url.UserId,
            url.ShortCode,
            url.OriginalUrl,
            url.CreatedAt,
            url.ExpiresAt);
    }

    public async Task<PagedResult<UserUrlResult>> GetUserUrls(
        int userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("A valid user is required.", nameof(userId));
        }

        if (page <= 0)
        {
            throw new ArgumentException("Page must be greater than zero.", nameof(page));
        }

        if (pageSize <= 0 || pageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100.", nameof(pageSize));
        }

        var totalCount = await urlRepository.GetCountByUserIdAsync(userId, cancellationToken);
        var urls = await urlRepository.GetByUserIdAsync(userId, page, pageSize, cancellationToken);

        var items = urls
            .Select(url => new UserUrlResult(
                url.Id,
                url.ShortCode,
                url.OriginalUrl,
                url.ClickCount,
                url.CreatedAt,
                url.ExpiresAt))
            .ToList();

        return new PagedResult<UserUrlResult>(items, totalCount, page, pageSize);
    }

    public async Task<UrlRedirectResult?> GetOriginalUrl(
        string shortCode,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            return null;
        }

        shortCode = shortCode.Trim();

        var cacheKey = $"url:{shortCode}";

// 1. Try cache
        var cachedUrl = await cacheService.GetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedUrl))
        {
            logger.LogInformation("Cache hit for short code {ShortCode}", shortCode);
            await TrackClick(shortCode, ipAddress, userAgent, cancellationToken);
            return new UrlRedirectResult(cachedUrl, false);
        }

        logger.LogInformation("Cache miss for short code {ShortCode}", shortCode);

        // 2. Fallback to DB
        var url = await urlRepository.GetByShortCodeAsync(shortCode, cancellationToken);
        if (url is null)
        {
            return null;
        }

        // Expiry check
        if (url.ExpiresAt.HasValue && url.ExpiresAt.Value <= DateTime.UtcNow)
        {
            return new UrlRedirectResult(url.OriginalUrl, true);
        }

        // 3. Store in cache
        await cacheService.SetAsync(cacheKey, url.OriginalUrl, TimeSpan.FromHours(24));
        await SaveClick(url, ipAddress, userAgent, cancellationToken);
        logger.LogInformation("Redirect resolved for short code {ShortCode} to {OriginalUrl}", shortCode, url.OriginalUrl);

        return new UrlRedirectResult(url.OriginalUrl, false);
    }

    public async Task<UrlAnalyticsResult?> GetUrlAnalytics(int userId, int urlId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("A valid user is required.", nameof(userId));
        }

        var url = await urlRepository.GetByIdAndUserIdAsync(urlId, userId, cancellationToken);
        if (url is null)
        {
            return null;
        }

        var recentClicks = await urlRepository.GetRecentClicksAsync(url.Id, 10, cancellationToken);

        return new UrlAnalyticsResult(
            url.Id,
            url.ShortCode,
            url.OriginalUrl,
            url.ClickCount,
            url.CreatedAt,
            recentClicks
                .Select(click => new UrlClickDetailResult(click.ClickedAt, click.IpAddress, click.UserAgent))
                .ToList());
    }

    public async Task DeleteUrl(int userId, int urlId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("A valid user is required.", nameof(userId));
        }

        var url = await urlRepository.GetByIdAndUserIdAsync(urlId, userId, cancellationToken);
        if (url is null)
        {
            throw new InvalidOperationException("URL was not found.");
        }

        await urlRepository.DeleteAsync(url, cancellationToken);
        await urlRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task TrackClick(string shortCode, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var url = await urlRepository.GetByShortCodeAsync(shortCode, cancellationToken);
        if (url is null)
        {
            return;
        }

        await SaveClick(url, ipAddress, userAgent, cancellationToken);
    }

    private async Task SaveClick(Url url, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        url.ClickCount++;

        await urlRepository.AddClickAsync(new UrlClick
        {
            UrlId = url.Id,
            ClickedAt = DateTime.UtcNow,
            IpAddress = TrimOrNull(ipAddress, 64),
            UserAgent = TrimOrNull(userAgent, 512)
        }, cancellationToken);

        await urlRepository.SaveChangesAsync(cancellationToken);
    }

    private static string? TrimOrNull(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
