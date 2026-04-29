namespace UrlShortener.API.Contracts;

public record UrlClickResponse(DateTime ClickedAt, string? IpAddress, string? UserAgent);
