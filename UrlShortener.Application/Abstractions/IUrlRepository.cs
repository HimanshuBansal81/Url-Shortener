using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions;

public interface IUrlRepository
{
    Task AddAsync(Url url, CancellationToken cancellationToken = default);

    Task<Url?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);

    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Url>> GetByUserIdAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> GetCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Url?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Url url, CancellationToken cancellationToken = default);

    Task AddClickAsync(UrlClick click, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UrlClick>> GetRecentClicksAsync(int urlId, int take, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
