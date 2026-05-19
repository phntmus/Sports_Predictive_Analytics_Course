using System.ComponentModel.DataAnnotations;

namespace SportsAnalyticsWeb.Models;

// Рынок спортивного события: П1, Х, П2 и т.д
public sealed class SportsMarket
{
    public int Id { get; set; }

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public ICollection<MarketOdds> Odds { get; set; } = new List<MarketOdds>();
}
