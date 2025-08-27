using Microsoft.EntityFrameworkCore;
using SteamBacklogTracker.Core.Entities;

namespace SteamBacklogTracker.Infrastructure.Data;

public class SteamBacklogContext : DbContext
{
    public SteamBacklogContext(DbContextOptions<SteamBacklogContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<GameTag> GameTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SteamAppId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Developer).HasMaxLength(200);
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(10, 2);

            entity.HasMany(g => g.Achievements)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Genres)
                .WithMany(ge => ge.Games);

            entity.HasMany(g => g.GameTags)
                .WithOne(gt => gt.Game)
                .HasForeignKey(gt => gt.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SteamAchievementId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<GameTag>(entity =>
        {
            entity.HasKey(e => new { e.GameId, e.TagId });
            
            entity.HasOne(gt => gt.Game)
                .WithMany(g => g.GameTags)
                .HasForeignKey(gt => gt.GameId);

            entity.HasOne(gt => gt.Tag)
                .WithMany(t => t.GameTags)
                .HasForeignKey(gt => gt.TagId);
        });
    }
}