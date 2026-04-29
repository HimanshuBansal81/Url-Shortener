using System.ComponentModel.DataAnnotations;

namespace UrlShortener.API.Contracts;

public record CreateShortUrlRequest(
    [Required] string OriginalUrl,
    string? CustomAlias,
    DateTime? ExpiresAt);
