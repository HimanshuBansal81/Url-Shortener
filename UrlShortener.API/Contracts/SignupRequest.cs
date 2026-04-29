using System.ComponentModel.DataAnnotations;

namespace UrlShortener.API.Contracts;

public record SignupRequest(
    [Required, MaxLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);
