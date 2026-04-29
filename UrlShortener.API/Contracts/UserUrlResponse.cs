namespace UrlShortener.API.Contracts;

public record UserUrlResponse(int Id, string ShortCode, string ShortUrl, string OriginalUrl, int ClickCount, DateTime CreatedAt, DateTime? ExpiresAt);
