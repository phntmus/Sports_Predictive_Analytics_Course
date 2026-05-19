using FluentValidation;
using SportsAnalyticsWeb.ViewModels;

namespace SportsAnalyticsWeb.Validators;

// Валидация формы ручного расчета прогноза 
public sealed class ManualPredictionRequestValidator : AbstractValidator<ManualPredictionRequest>
{
    public ManualPredictionRequestValidator()
    {
        RuleFor(x => x.HomeTeamId).GreaterThan(0);
        RuleFor(x => x.AwayTeamId).GreaterThan(0);
        RuleFor(x => x).Must(x => x.HomeTeamId != x.AwayTeamId).WithMessage("Команды должны отличаться.");
        RuleFor(x => x.MarginPercent).InclusiveBetween(0, 20);
    }
}
