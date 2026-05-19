namespace SportsAnalyticsWeb.Models;

// DTO результата прогноза
public sealed record PredictionResult(
    int MatchId,
    string MatchName,
    double HomeWinProbability,
    double DrawProbability,
    double AwayWinProbability,
    decimal HomeWinOdds,
    decimal DrawOdds,
    decimal AwayWinOdds,
    string Explanation);
