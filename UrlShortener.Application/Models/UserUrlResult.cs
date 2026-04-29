namespace UrlShortener.Application.Models;

public record UserUrlResult(int Id, string ShortCode, string OriginalUrl, int ClickCount, DateTime CreatedAt, DateTime? ExpiresAt);
