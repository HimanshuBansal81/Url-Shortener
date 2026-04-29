using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Url> Urls => Set<Url>();

    public DbSet<UrlClick> UrlClicks => Set<UrlClick>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id)
                .ValueGeneratedOnAdd();

            entity.Property(user => user.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(user => user.PasswordHash)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Url>(entity =>
            {
                entity.HasKey(url => url.Id);

                entity.Property(url => url.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(url => url.ShortCode)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.Property(url => url.OriginalUrl)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(url => url.CreatedAt)
                    .IsRequired();

                entity.HasIndex(url => url.ShortCode)
                    .IsUnique();

                entity.HasIndex(url => url.UserId);

                entity.HasIndex(url => url.CreatedAt);

                entity.HasOne(url => url.User)
                    .WithMany(user => user.Urls)
                    .HasForeignKey(url => url.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder.Entity<UrlClick>(entity =>
        {
            entity.HasKey(click => click.Id);

            entity.Property(click => click.Id)
                .ValueGeneratedOnAdd();

            entity.Property(click => click.ClickedAt)
                .IsRequired();

            entity.Property(click => click.IpAddress)
                .HasMaxLength(64);

            entity.Property(click => click.UserAgent)
                .HasMaxLength(512);

            entity.HasIndex(click => click.UrlId);

            entity.HasIndex(click => click.ClickedAt);

            entity.HasOne(click => click.Url)
                .WithMany(url => url.Clicks)
                .HasForeignKey(click => click.UrlId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
