namespace UrlShortener.API.Contracts;

public record CreateShortUrlResponse(int Id, int UserId, string ShortCode, string ShortUrl, string OriginalUrl, DateTime CreatedAt, DateTime? ExpiresAt);
