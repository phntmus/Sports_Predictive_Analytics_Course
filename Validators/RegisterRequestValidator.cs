using FluentValidation;
using SportsAnalyticsWeb.ViewModels;

namespace SportsAnalyticsWeb.Validators;

// Правила валидации формы регистрации
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Некорректный формат Email.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Имя обязательно.")
            .MinimumLength(2).WithMessage("Имя должно содержать минимум 2 символа.")
            .MaximumLength(60).WithMessage("Имя не должно превышать 60 символов.");

        // Ключевое правило: пользователю должно быть не менее 18 лет
        RuleFor(x => x.DateOfBirth)
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Вам должно быть не менее 18 лет для регистрации.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Пароли не совпадают.");
    }

    // Проверяет что пользователю исполнилось 18 лет
    private static bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        // Корректировка если день рождения ещё не наступил в этом году
        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }
}
