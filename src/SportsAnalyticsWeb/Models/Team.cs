using System.ComponentModel.DataAnnotations;

namespace SportsAnalyticsWeb.Models;

// Спортивная команда
public sealed class Team
{
    public int Id { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(60)]
    public string League { get; set; } = string.Empty;

    public double EloRating { get; set; } = 1500;

    // Путь к SVG-гербу команды (относительно wwwroot)
    [MaxLength(200)]
    public string LogoUrl { get; set; } = string.Empty;

    public TeamStatistic? Statistic { get; set; }

    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}