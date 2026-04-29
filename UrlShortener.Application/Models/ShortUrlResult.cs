namespace UrlShortener.Application.Models;

public record ShortUrlResult(int Id, int UserId, string ShortCode, string OriginalUrl, DateTime CreatedAt, DateTime? ExpiresAt);
