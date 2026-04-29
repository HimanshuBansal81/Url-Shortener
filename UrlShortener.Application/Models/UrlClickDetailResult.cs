namespace UrlShortener.Application.Models;

public record UrlClickDetailResult(DateTime ClickedAt, string? IpAddress, string? UserAgent);
