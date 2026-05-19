namespace SportsAnalyticsWeb.Models;

// Исторический матч с предыдущими рыночными коэффициентами
public sealed class HistoricalMatch
{
    public int Id { get; set; }

    public int HomeTeamId { get; set; }

    public Team? HomeTeam { get; set; }

    public int AwayTeamId { get; set; }

    public Team? AwayTeam { get; set; }

    public DateTime PlayedAt { get; set; }

    public int HomeGoals { get; set; }

    public int AwayGoals { get; set; }

    public decimal HomeOdds { get; set; }

    public decimal DrawOdds { get; set; }

    public decimal AwayOdds { get; set; }
}
