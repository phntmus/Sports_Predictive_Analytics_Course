using SportsAnalyticsWeb.Services;
using Xunit;

namespace SportsAnalytics.Tests;

public sealed class OddsPredictionServiceTests
{
    [Fact]
    public void CalculateEloProbability_WhenRatingsAreEqual_ReturnsHalf()
    {
        var result = OddsPredictionService.CalculateEloProbability(1500, 1500);
        Assert.InRange(result, 0.499, 0.501);
    }

    [Fact]
    public void CalculateEloProbability_WhenHomeRatingIsHigher_ReturnsMoreThanHalf()
    {
        var result = OddsPredictionService.CalculateEloProbability(1700, 1500);
        Assert.True(result > 0.5);
    }

    [Fact]
    public void ToOdds_WithSevenPercentMargin_ReturnsLowerThanFairOdds()
    {
        var fairOdds = (decimal)(1 / 0.5);
        var odds = OddsPredictionService.ToOdds(0.5, 7);
        Assert.True(odds < fairOdds);
    }

    [Fact]
    public void ToOdds_ForProbabilityThirtyPercent_ReturnsPositiveValue()
    {
        var odds = OddsPredictionService.ToOdds(0.30, 7);
        Assert.True(odds > 0);
    }
}
