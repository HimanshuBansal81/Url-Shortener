using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Contracts;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/url")]
public class UrlController(IUrlService urlService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("create-url")]
    public async Task<ActionResult<ApiResponse<CreateShortUrlResponse>>> CreateShortUrl(
        [FromBody] CreateShortUrlRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await urlService.CreateShortUrl(
            userId,
            request.OriginalUrl,
            request.CustomAlias,
            request.ExpiresAt,
            cancellationToken);
        var shortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortCode}";
        var response = new CreateShortUrlResponse(
            result.Id,
            result.UserId,
            result.ShortCode,
            shortUrl,
            result.OriginalUrl,
            result.CreatedAt,
            result.ExpiresAt);

        return Created(shortUrl, new ApiResponse<CreateShortUrlResponse>(true, response, "Short URL created successfully."));
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserUrlResponse>>>> GetDashboard(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var results = await urlService.GetUserUrls(userId, page, pageSize, cancellationToken);

        var response = new PagedResponse<UserUrlResponse>(
            results.Items.Select(result => new UserUrlResponse(
                result.Id,
                result.ShortCode,
                $"{Request.Scheme}://{Request.Host}/{result.ShortCode}",
                result.OriginalUrl,
                result.ClickCount,
                result.CreatedAt,
                result.ExpiresAt))
            .ToList(),
            results.TotalCount,
            results.Page,
            results.PageSize);

        return Ok(new ApiResponse<PagedResponse<UserUrlResponse>>(true, response));
    }

    [HttpGet("{id:int}/analytics")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UrlAnalyticsResponse>>> GetAnalytics(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await urlService.GetUrlAnalytics(userId, id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(new ApiResponse<UrlAnalyticsResponse>(true, new UrlAnalyticsResponse(
            result.UrlId,
            result.ShortCode,
            $"{Request.Scheme}://{Request.Host}/{result.ShortCode}",
            result.OriginalUrl,
            result.ClickCount,
            result.CreatedAt,
            result.RecentClicks
                .Select(click => new UrlClickResponse(click.ClickedAt, click.IpAddress, click.UserAgent))
                .ToList())));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUrl(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await urlService.DeleteUrl(userId, id, cancellationToken);
        return Ok(new ApiResponse<object>(true, new { id }, "URL deleted successfully."));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Authenticated user id is missing.");
        }

        return userId;
    }
}
