namespace UrlShortener.API.Contracts;

public record UrlAnalyticsResponse(
    int UrlId,
    string ShortCode,
    string ShortUrl,
    string OriginalUrl,
    int ClickCount,
    DateTime CreatedAt,
    IReadOnlyList<UrlClickResponse> RecentClicks);
