using System.ComponentModel.DataAnnotations;

namespace SportsAnalyticsWeb.Models;

// Предстоящий спортивный матч
public sealed class Match
{
    public int Id { get; set; }

    public int HomeTeamId { get; set; }

    public Team? HomeTeam { get; set; }

    public int AwayTeamId { get; set; }

    public Team? AwayTeam { get; set; }

    [MaxLength(60)]
    public string Sport { get; set; } = "Футбол";

    public DateTime StartTime { get; set; }

    public ICollection<MarketOdds> MarketOdds { get; set; } = new List<MarketOdds>();

    public Prediction? Prediction { get; set; }
}
