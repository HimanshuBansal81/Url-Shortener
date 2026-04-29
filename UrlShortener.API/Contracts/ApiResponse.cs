namespace UrlShortener.API.Contracts;

public record ApiResponse<T>(bool Success, T Data, string? Message = null);
