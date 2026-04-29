using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}
