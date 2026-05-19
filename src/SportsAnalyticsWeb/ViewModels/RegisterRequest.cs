namespace SportsAnalyticsWeb.ViewModels;

// Данные формы регистрации нового пользователя
public sealed class RegisterRequest
{
    // Email — используется как логин
    public string Email { get; set; } = string.Empty;
    // Отображаемое имя
    public string DisplayName { get; set; } = string.Empty;
    // Дата рождения — должна подтверждать возраст от 18 лет
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);
    // Пароль
    public string Password { get; set; } = string.Empty;
    // Подтверждение пароля
    public string ConfirmPassword { get; set; } = string.Empty;
}
