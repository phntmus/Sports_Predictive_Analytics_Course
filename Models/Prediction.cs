namespace SportsAnalyticsWeb.Models;

// Итоговый прогноз для матча 
public sealed class Prediction
{
    public int Id { get; set; }

    public int MatchId { get; set; }

    public Match? Match { get; set; }

    public double HomeWinProbability { get; set; }

    public double DrawProbability { get; set; }

    public double AwayWinProbability { get; set; }

    public decimal HomeWinOdds { get; set; }

    public decimal DrawOdds { get; set; }

    public decimal AwayWinOdds { get; set; }

    public double MarginPercent { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
