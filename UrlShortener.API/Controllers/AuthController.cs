using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Contracts;
using UrlShortener.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Signup([FromBody] SignupRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Signup attempt for email {Email}", request.Email);
        var result = await authService.SignupAsync(request.Name, request.Email, request.Password, cancellationToken);
        return Ok(new ApiResponse<AuthResponse>(true, new AuthResponse(result.UserId, result.Name, result.Email, result.Token), "Signup successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email {Email}", request.Email);
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return Ok(new ApiResponse<AuthResponse>(true, new AuthResponse(result.UserId, result.Name, result.Email, result.Token), "Login successful."));
    }
}
