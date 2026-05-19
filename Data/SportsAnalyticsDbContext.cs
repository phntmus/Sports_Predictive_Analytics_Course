using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Data;

// EF Core DbContext приложения
public sealed class SportsAnalyticsDbContext : IdentityDbContext<ApplicationUser>
{
    public SportsAnalyticsDbContext(DbContextOptions<SportsAnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamStatistic> TeamStatistics => Set<TeamStatistic>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<HistoricalMatch> HistoricalMatches => Set<HistoricalMatch>();
    public DbSet<SportsMarket> SportsMarkets => Set<SportsMarket>();
    public DbSet<MarketOdds> MarketOdds => Set<MarketOdds>();
    public DbSet<Prediction> Predictions => Set<Prediction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Team>()
            .HasIndex(x => x.Name)
            .IsUnique();

        // Связь 1:1 — Team и TeamStatistic
        builder.Entity<Team>()
            .HasOne(x => x.Statistic)
            .WithOne(x => x.Team)
            .HasForeignKey<TeamStatistic>(x => x.TeamId);

        // Связь 1:N — Match с HomeTeam
        builder.Entity<Match>()
            .HasOne(x => x.HomeTeam)
            .WithMany(x => x.HomeMatches)
            .HasForeignKey(x => x.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь 1:N — Match с AwayTeam
        builder.Entity<Match>()
            .HasOne(x => x.AwayTeam)
            .WithMany(x => x.AwayMatches)
            .HasForeignKey(x => x.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь N:N — Match и SportsMarket через MarketOdds (составной ключ)
        builder.Entity<MarketOdds>()
            .HasKey(x => new { x.MatchId, x.SportsMarketId });

        builder.Entity<MarketOdds>()
            .HasOne(x => x.Match)
            .WithMany(x => x.MarketOdds)
            .HasForeignKey(x => x.MatchId);

        builder.Entity<MarketOdds>()
            .HasOne(x => x.SportsMarket)
            .WithMany(x => x.Odds)
            .HasForeignKey(x => x.SportsMarketId);

        // Связь 1:1 — Prediction и Match
        builder.Entity<Prediction>()
            .HasOne(x => x.Match)
            .WithOne(x => x.Prediction)
            .HasForeignKey<Prediction>(x => x.MatchId);
    }
}
