namespace UrlShortener.Domain.Entities;

public class Url
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ShortCode { get; set; } = null!;

    public string OriginalUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public int ClickCount { get; set; } = 0;

    public User User { get; set; } = null!;

    public ICollection<UrlClick> Clicks { get; set; } = new List<UrlClick>();
}
