namespace UrlShortener.API.Contracts;

public record AuthResponse(int UserId, string Name, string Email, string Token);
