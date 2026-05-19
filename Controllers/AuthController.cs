using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsAnalyticsWeb.Models;
using SportsAnalyticsWeb.ViewModels;

namespace SportsAnalyticsWeb.Controllers;

// MVC-контроллер аутентификации.
// SignInManager требует настоящий HttpContext для записи cookie —
// поэтому auth-операции вынесены из Blazor-компонентов в этот контроллер.
[Route("auth")]
public sealed class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IValidator<RegisterRequest> registerValidator;
    private readonly IValidator<LoginRequest> loginValidator;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.registerValidator = registerValidator;
        this.loginValidator = loginValidator;
    }

    // GET /auth/login — показывает страницу входа (Blazor)
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        // Перенаправляем на Blazor-страницу /login
        return Redirect("/login");
    }

    // POST /auth/login — обрабатывает форму входа
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginPost([FromForm] LoginRequest model, [FromForm] string? returnUrl = null)
    {
        var validation = await this.loginValidator.ValidateAsync(model);
        if (!validation.IsValid)
        {
            var errors = string.Join("|", validation.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}"));
            return Redirect($"/login?error={Uri.EscapeDataString(errors)}");
        }

        var result = await this.signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }

        return Redirect("/login?error=invalid");
    }

    // POST /auth/register — обрабатывает форму регистрации
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterPost([FromForm] RegisterRequest model)
    {
        var validation = await this.registerValidator.ValidateAsync(model);
        if (!validation.IsValid)
        {
            var errors = string.Join("|", validation.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}"));
            return Redirect($"/register?error={Uri.EscapeDataString(errors)}");
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName,
            DateOfBirth = model.DateOfBirth,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return Redirect($"/register?error={Uri.EscapeDataString(errors)}");
        }

        await this.signInManager.SignInAsync(user, isPersistent: false);
        return Redirect("/");
    }

    // POST /auth/logout — выход из системы
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        await this.signInManager.SignOutAsync();
        return Redirect("/login");
    }

    // GET /auth/logout — выход через GET (для NavLink в Blazor)
    [HttpGet("logout")]
    public async Task<IActionResult> LogoutGet()
    {
        await this.signInManager.SignOutAsync();
        return Redirect("/login");
    }
}
