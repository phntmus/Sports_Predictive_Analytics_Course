namespace SportsAnalyticsWeb.Models;

// Связь N:N между матчем и рынком с расчетным коэффициентом
public sealed class MarketOdds
{
    public int MatchId { get; set; }

    public Match? Match { get; set; }

    public int SportsMarketId { get; set; }

    public SportsMarket? SportsMarket { get; set; }

    public double Probability { get; set; }

    public decimal Odds { get; set; }
}
