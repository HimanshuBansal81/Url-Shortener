using UrlShortener.Application.Models;

namespace UrlShortener.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResult> SignupAsync(string name, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
