using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("")]
public class RedirectController(IUrlService urlService) : ControllerBase
{
    [HttpGet("{shortCode}")]
    [AllowAnonymous]
    [EnableRateLimiting("redirect")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortCode, CancellationToken cancellationToken)
    {
        var result = await urlService.GetOriginalUrl(
            shortCode,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        if (result.IsExpired)
        {
            return NotFound("This short URL has expired.");
        }

        return Redirect(result.OriginalUrl);
    }
}
