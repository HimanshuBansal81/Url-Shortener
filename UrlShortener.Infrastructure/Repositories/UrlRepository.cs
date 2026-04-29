using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlRepository(AppDbContext dbContext) : IUrlRepository
{
    public Task AddAsync(Url url, CancellationToken cancellationToken = default) =>
        dbContext.Urls.AddAsync(url, cancellationToken).AsTask();

    public Task AddClickAsync(UrlClick click, CancellationToken cancellationToken = default) =>
        dbContext.UrlClicks.AddAsync(click, cancellationToken).AsTask();

    public Task<Url?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default) =>
        dbContext.Urls.FirstOrDefaultAsync(url => url.ShortCode == shortCode, cancellationToken);

    public Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default) =>
        dbContext.Urls.AnyAsync(url => url.ShortCode == shortCode, cancellationToken);

    public async Task<IReadOnlyList<Url>> GetByUserIdAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await dbContext.Urls
            .Where(url => url.UserId == userId)
            .OrderByDescending(url => url.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public Task<int> GetCountByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        dbContext.Urls.CountAsync(url => url.UserId == userId, cancellationToken);

    public Task<Url?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default) =>
        dbContext.Urls.FirstOrDefaultAsync(url => url.Id == id && url.UserId == userId, cancellationToken);

    public Task DeleteAsync(Url url, CancellationToken cancellationToken = default)
    {
        dbContext.Urls.Remove(url);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<UrlClick>> GetRecentClicksAsync(int urlId, int take, CancellationToken cancellationToken = default) =>
        await dbContext.UrlClicks
            .Where(click => click.UrlId == urlId)
            .OrderByDescending(click => click.ClickedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
