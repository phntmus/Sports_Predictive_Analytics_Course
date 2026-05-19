using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsAnalyticsWeb.Components;
using SportsAnalyticsWeb.Data;
using SportsAnalyticsWeb.Models;
using SportsAnalyticsWeb.Repositories;
using SportsAnalyticsWeb.Services;
using SportsAnalyticsWeb.Validators;
using SportsAnalyticsWeb.ViewModels;

namespace SportsAnalyticsWeb;

// Точка входа приложения 424 Sport Predict 
public sealed class Program
{
    // Главный метод. Конфигурирует DI, middleware и запускает веб-сервер
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigurePipeline(app);

        await InitialiseDatabaseAsync(app);

        await app.RunAsync();
    }

    // Регистрация всех сервисов в DI-контейнере
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Blazor Server с интерактивным SignalR-режимом
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddCascadingAuthenticationState();

        // SQLite — файловая БД, не требует отдельного сервера
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        builder.Services.AddDbContext<SportsAnalyticsDbContext>(options =>
            options.UseSqlite(connectionString));

        // ASP.NET Core Identity — аутентификация и авторизация пользователей
        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<SportsAnalyticsDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Аутентификация через cookie.
        // ConfigureApplicationCookie — правильный способ задать LoginPath в .NET 8.
        // AddIdentityCookies() с лямбдой работает иначе и не переопределяет путь.
        builder.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            // По умолчанию Identity редиректит на /Account/Login — это вызывало 404.
            // Указываем путь нашего MVC-контроллера аутентификации.
            options.LoginPath = "/auth/login";
            options.LogoutPath = "/auth/logout";
            options.AccessDeniedPath = "/auth/login";
        });

        builder.Services.AddAuthorization();

        // Репозитории — паттерн Repository
        builder.Services.AddScoped<ITeamRepository, TeamRepository>();
        builder.Services.AddScoped<IMatchRepository, MatchRepository>();

        // Сервис бизнес-логики
        builder.Services.AddScoped<IOddsPredictionService, OddsPredictionService>();

        // Валидаторы FluentValidation
        builder.Services.AddScoped<IValidator<ManualPredictionRequest>, ManualPredictionRequestValidator>();
        builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        // In-memory кеш
        builder.Services.AddMemoryCache();
        builder.Services.AddDistributedMemoryCache();

        // MVC-контроллеры (нужны для AuthController) + Views для Razor Pages
        builder.Services.AddControllersWithViews();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddHealthChecks();
    }

    // Настройка middleware-конвейера
    private static void ConfigurePipeline(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    }

    // Инициализация БД: миграции и начальные данные
    private static async Task InitialiseDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<SportsAnalyticsDbContext>();

        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        await DbInitializer.SeedAsync(scope.ServiceProvider);
    }
}
