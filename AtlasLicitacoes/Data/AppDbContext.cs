using AtlasLicitacoes.Models.Portfolio;
using Microsoft.EntityFrameworkCore;

namespace AtlasLicitacoes.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OpportunityCache> Opportunities => Set<OpportunityCache>();
    public DbSet<WatchlistItem> Watchlist => Set<WatchlistItem>();
    public DbSet<PortfolioProduct> PortfolioProducts => Set<PortfolioProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpportunityCache>().HasIndex(x => x.SourceKey).IsUnique();
        modelBuilder.Entity<PortfolioProduct>().HasIndex(x => x.Sku).IsUnique();
    }
}

public sealed class OpportunityCache
{
    public int Id { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string SourceKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string MatchTags { get; set; } = string.Empty;
    public string RawJson { get; set; } = string.Empty;
    public DateTimeOffset SyncedAt { get; set; }
}

public sealed class WatchlistItem
{
    public int Id { get; set; }
    public string SourceKey { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
