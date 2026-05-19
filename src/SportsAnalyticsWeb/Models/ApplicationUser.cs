using Microsoft.AspNetCore.Identity;

namespace SportsAnalyticsWeb.Models;

// Пользователь системы 424 Sport Predict
public sealed class ApplicationUser : IdentityUser
{
    // Отображаемое имя пользователя
    public string DisplayName { get; set; } = string.Empty;

    // Дата рождения. Используется для проверки возраста (18+)
    public DateTime DateOfBirth { get; set; }
}
