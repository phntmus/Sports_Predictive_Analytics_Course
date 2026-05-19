namespace SportsAnalyticsWeb.Models;

// Агрегированная статистика команды. Связь 1:1 с Team
public sealed class TeamStatistic
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public Team? Team { get; set; }

    public int MatchesPlayed { get; set; }

    public int Wins { get; set; }

    public int Draws { get; set; }

    public int Losses { get; set; }

    public int GoalsFor { get; set; }

    public int GoalsAgainst { get; set; }
}
