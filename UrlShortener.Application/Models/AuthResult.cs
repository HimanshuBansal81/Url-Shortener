namespace UrlShortener.Application.Models;

public record AuthResult(int UserId, string Name, string Email, string Token);
