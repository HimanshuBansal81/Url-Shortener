using System.ComponentModel.DataAnnotations;

namespace UrlShortener.API.Contracts;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
