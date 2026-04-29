namespace UrlShortener.Domain.Entities;

public class UrlClick
{
    public int Id { get; set; }

    public int UrlId { get; set; }

    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public Url Url { get; set; } = null!;
}
