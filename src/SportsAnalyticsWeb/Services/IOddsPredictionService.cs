using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Services;

public interface IOddsPredictionService
{
    Task<List<PredictionResult>> GetUpcomingPredictionsAsync();
    Task<PredictionResult> CalculateForMatchAsync(int matchId);
    PredictionResult CalculateManual(Team homeTeam, Team awayTeam, IEnumerable<HistoricalMatch> history, double marginPercent);
}
