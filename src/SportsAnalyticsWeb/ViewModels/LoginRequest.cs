namespace SportsAnalyticsWeb.ViewModels;

// Данные формы входа в систему
public sealed class LoginRequest
{
    // Email пользователя
    public string Email { get; set; } = string.Empty;
    // Пароль пользователя 
    public string Password { get; set; } = string.Empty;
    // Запомнить сессию
    public bool RememberMe { get; set; }
}
