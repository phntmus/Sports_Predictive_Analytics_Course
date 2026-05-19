namespace SportsAnalyticsWeb.ViewModels;

// Запрос для ручного расчета коэффициентов
public sealed class ManualPredictionRequest
{
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public double MarginPercent { get; set; } = 7;
}
