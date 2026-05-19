using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsAnalyticsWeb.Models;
using SportsAnalyticsWeb.Services;

namespace SportsAnalyticsWeb.Data;

// Заполнение базы начальными данными для демонстрации
public static class DbInitializer
{
    // Вставляет начальные данные если БД пуста
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<SportsAnalyticsDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var predictionService = serviceProvider.GetRequiredService<IOddsPredictionService>();

        if (!await userManager.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@sport.local",
                Email = "admin@sport.local",
                DisplayName = "Администратор",
                DateOfBirth = new DateTime(1990, 1, 1),
            };
            await userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await db.Teams.AnyAsync())
        {
            var teams = new[]
            {
                new Team { Name = "Зенит",     League = "РПЛ", EloRating = 1685, LogoUrl = "/img/crests/zenit.svg" },
                new Team { Name = "Спартак",   League = "РПЛ", EloRating = 1590, LogoUrl = "/img/crests/spartak.svg" },
                new Team { Name = "ЦСКА",      League = "РПЛ", EloRating = 1610, LogoUrl = "/img/crests/cska.svg" },
                new Team { Name = "Динамо",    League = "РПЛ", EloRating = 1575, LogoUrl = "/img/crests/dynamo.svg" },
                new Team { Name = "Локомотив", League = "РПЛ", EloRating = 1555, LogoUrl = "/img/crests/lokomotiv.svg" },
                new Team { Name = "Краснодар", League = "РПЛ", EloRating = 1640, LogoUrl = "/img/crests/krasnodar.svg" },
            };
            db.Teams.AddRange(teams);
            await db.SaveChangesAsync();

            db.TeamStatistics.AddRange(
                new TeamStatistic { TeamId = teams[0].Id, MatchesPlayed = 10, Wins = 7, Draws = 2, Losses = 1, GoalsFor = 24, GoalsAgainst = 9 },
                new TeamStatistic { TeamId = teams[1].Id, MatchesPlayed = 10, Wins = 5, Draws = 2, Losses = 3, GoalsFor = 18, GoalsAgainst = 14 },
                new TeamStatistic { TeamId = teams[2].Id, MatchesPlayed = 10, Wins = 5, Draws = 3, Losses = 2, GoalsFor = 16, GoalsAgainst = 10 },
                new TeamStatistic { TeamId = teams[3].Id, MatchesPlayed = 10, Wins = 4, Draws = 3, Losses = 3, GoalsFor = 14, GoalsAgainst = 12 },
                new TeamStatistic { TeamId = teams[4].Id, MatchesPlayed = 10, Wins = 4, Draws = 2, Losses = 4, GoalsFor = 13, GoalsAgainst = 13 },
                new TeamStatistic { TeamId = teams[5].Id, MatchesPlayed = 10, Wins = 6, Draws = 2, Losses = 2, GoalsFor = 20, GoalsAgainst = 11 });

            db.SportsMarkets.AddRange(
                new SportsMarket { Code = "P1", Name = "Победа хозяев" },
                new SportsMarket { Code = "X",  Name = "Ничья" },
                new SportsMarket { Code = "P2", Name = "Победа гостей" });

            db.HistoricalMatches.AddRange(
                new HistoricalMatch { HomeTeamId = teams[0].Id, AwayTeamId = teams[1].Id, PlayedAt = DateTime.UtcNow.AddDays(-40), HomeGoals = 2, AwayGoals = 1, HomeOdds = 1.75m, DrawOdds = 3.40m, AwayOdds = 4.80m },
                new HistoricalMatch { HomeTeamId = teams[1].Id, AwayTeamId = teams[2].Id, PlayedAt = DateTime.UtcNow.AddDays(-35), HomeGoals = 1, AwayGoals = 1, HomeOdds = 2.35m, DrawOdds = 3.10m, AwayOdds = 3.00m },
                new HistoricalMatch { HomeTeamId = teams[5].Id, AwayTeamId = teams[3].Id, PlayedAt = DateTime.UtcNow.AddDays(-30), HomeGoals = 3, AwayGoals = 1, HomeOdds = 2.05m, DrawOdds = 3.20m, AwayOdds = 3.60m },
                new HistoricalMatch { HomeTeamId = teams[2].Id, AwayTeamId = teams[4].Id, PlayedAt = DateTime.UtcNow.AddDays(-24), HomeGoals = 2, AwayGoals = 0, HomeOdds = 1.95m, DrawOdds = 3.25m, AwayOdds = 3.90m },
                new HistoricalMatch { HomeTeamId = teams[3].Id, AwayTeamId = teams[0].Id, PlayedAt = DateTime.UtcNow.AddDays(-18), HomeGoals = 0, AwayGoals = 2, HomeOdds = 3.70m, DrawOdds = 3.30m, AwayOdds = 1.95m },
                new HistoricalMatch { HomeTeamId = teams[4].Id, AwayTeamId = teams[5].Id, PlayedAt = DateTime.UtcNow.AddDays(-12), HomeGoals = 1, AwayGoals = 2, HomeOdds = 2.80m, DrawOdds = 3.15m, AwayOdds = 2.45m });

            db.Matches.AddRange(
                new Match { HomeTeamId = teams[0].Id, AwayTeamId = teams[5].Id, Sport = "Футбол", StartTime = DateTime.UtcNow.AddDays(3) },
                new Match { HomeTeamId = teams[1].Id, AwayTeamId = teams[3].Id, Sport = "Футбол", StartTime = DateTime.UtcNow.AddDays(4) },
                new Match { HomeTeamId = teams[2].Id, AwayTeamId = teams[4].Id, Sport = "Футбол", StartTime = DateTime.UtcNow.AddDays(5) });

            await db.SaveChangesAsync();
        }

        foreach (var match in await db.Matches.AsNoTracking().ToListAsync())
        {
            await predictionService.CalculateForMatchAsync(match.Id);
        }
    }
}
