using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Models;
using UrlShortener.Application.Validators;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthResult> SignupAsync(string name, string email, string password, CancellationToken cancellationToken = default)
    {
        AuthValidator.ValidateSignup(name, email, password);

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return new AuthResult(user.Id, user.Name, user.Email, tokenGenerator.GenerateToken(user));
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        AuthValidator.ValidateLogin(email, password);

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return new AuthResult(user.Id, user.Name, user.Email, tokenGenerator.GenerateToken(user));
    }
}
