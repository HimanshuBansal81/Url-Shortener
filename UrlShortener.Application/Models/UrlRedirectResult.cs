namespace UrlShortener.Application.Models;

public record UrlRedirectResult(string OriginalUrl, bool IsExpired);
