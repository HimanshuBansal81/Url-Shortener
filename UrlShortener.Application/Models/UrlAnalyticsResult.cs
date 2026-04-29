namespace UrlShortener.Application.Models;

public record UrlAnalyticsResult(
    int UrlId,
    string ShortCode,
    string OriginalUrl,
    int ClickCount,
    DateTime CreatedAt,
    IReadOnlyList<UrlClickDetailResult> RecentClicks);
