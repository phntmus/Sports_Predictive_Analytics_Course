using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SportsAnalyticsWeb.Data;
using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Services;

// Сервис предиктивной аналитики и расчета коэффициентов
public sealed class OddsPredictionService : IOddsPredictionService
{
    private readonly SportsAnalyticsDbContext db;
    private readonly IConfiguration configuration;
    private readonly IDistributedCache cache;

    public OddsPredictionService(SportsAnalyticsDbContext db, IConfiguration configuration, IDistributedCache cache)
    {
        this.db = db;
        this.configuration = configuration;
        this.cache = cache;
    }

    public async Task<List<PredictionResult>> GetUpcomingPredictionsAsync()
    {
        var cacheKey = "upcoming-predictions";
        var cached = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            return JsonSerializer.Deserialize<List<PredictionResult>>(cached) ?? new List<PredictionResult>();
        }

        var matches = await db.Matches
            .Include(x => x.HomeTeam).ThenInclude(x => x!.Statistic)
            .Include(x => x.AwayTeam).ThenInclude(x => x!.Statistic)
            .Include(x => x.Prediction)
            .OrderBy(x => x.StartTime)
            .ToListAsync();

        var results = new List<PredictionResult>();
        foreach (var match in matches)
        {
            results.Add(await CalculateForMatchAsync(match.Id));
        }

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(results), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return results;
    }

    public async Task<PredictionResult> CalculateForMatchAsync(int matchId)
    {
        var match = await db.Matches
            .Include(x => x.HomeTeam).ThenInclude(x => x!.Statistic)
            .Include(x => x.AwayTeam).ThenInclude(x => x!.Statistic)
            .FirstAsync(x => x.Id == matchId);

        var history = await db.HistoricalMatches
            .Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .Where(x => x.HomeTeamId == match.HomeTeamId || x.AwayTeamId == match.HomeTeamId || x.HomeTeamId == match.AwayTeamId || x.AwayTeamId == match.AwayTeamId)
            .OrderByDescending(x => x.PlayedAt)
            .Take(20)
            .ToListAsync();

        var margin = configuration.GetValue<double>("Prediction:BookmakerMarginPercent", 7);
        var result = CalculateManual(match.HomeTeam!, match.AwayTeam!, history, margin);

        var prediction = await db.Predictions.FirstOrDefaultAsync(x => x.MatchId == matchId);
        if (prediction is null)
        {
            prediction = new Prediction { MatchId = matchId };
            db.Predictions.Add(prediction);
        }

        prediction.HomeWinProbability = result.HomeWinProbability;
        prediction.DrawProbability = result.DrawProbability;
        prediction.AwayWinProbability = result.AwayWinProbability;
        prediction.HomeWinOdds = result.HomeWinOdds;
        prediction.DrawOdds = result.DrawOdds;
        prediction.AwayWinOdds = result.AwayWinOdds;
        prediction.MarginPercent = margin;
        prediction.CalculatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await cache.RemoveAsync("upcoming-predictions");

        return result with { MatchId = matchId };
    }

    public PredictionResult CalculateManual(Team homeTeam, Team awayTeam, IEnumerable<HistoricalMatch> history, double marginPercent)
    {
        var eloHome = CalculateEloProbability(homeTeam.EloRating, awayTeam.EloRating);
        var eloAway = 1 - eloHome;
        var formHome = CalculateFormScore(homeTeam.Statistic);
        var formAway = CalculateFormScore(awayTeam.Statistic);
        var formTotal = Math.Max(formHome + formAway, 0.001);
        var formHomeProbability = formHome / formTotal;
        var formAwayProbability = formAway / formTotal;
        var market = CalculateHistoricalMarketProbability(history);

        var eloWeight = configuration.GetValue<double>("Prediction:EloWeight", 0.45);
        var formWeight = configuration.GetValue<double>("Prediction:FormWeight", 0.30);
        var marketWeight = configuration.GetValue<double>("Prediction:MarketWeight", 0.25);

        var rawHome = (eloWeight * eloHome) + (formWeight * formHomeProbability) + (marketWeight * market.home);
        var rawAway = (eloWeight * eloAway) + (formWeight * formAwayProbability) + (marketWeight * market.away);
        var rawDraw = 0.18 + (0.15 * market.draw);

        var sum = rawHome + rawDraw + rawAway;
        var home = rawHome / sum;
        var draw = rawDraw / sum;
        var away = rawAway / sum;

        return new PredictionResult(
            0,
            $"{homeTeam.Name} — {awayTeam.Name}",
            home,
            draw,
            away,
            ToOdds(home, marginPercent),
            ToOdds(draw, marginPercent),
            ToOdds(away, marginPercent),
            "Модель использует ELO-рейтинг, форму команды и нормализованную вероятность из прошлых коэффициентов.");
    }

    // Расчет вероятности победы по формуле ELO
    public static double CalculateEloProbability(double ratingA, double ratingB)
    {
        return 1.0 / (1.0 + Math.Pow(10.0, (ratingB - ratingA) / 400.0));
    }

    // Преобразование вероятности в коэффициент с учетом маржи 
    public static decimal ToOdds(double probability, double marginPercent)
    {
        var adjustedProbability = probability * (1.0 + (marginPercent / 100.0));
        return Math.Round((decimal)(1.0 / adjustedProbability), 2);
    }

    private static double CalculateFormScore(TeamStatistic? stat)
    {
        if (stat is null || stat.MatchesPlayed == 0)
        {
            return 0.5;
        }

        var points = (stat.Wins * 3) + stat.Draws;
        var maxPoints = stat.MatchesPlayed * 3.0;
        var goalBalanceBonus = Math.Clamp((stat.GoalsFor - stat.GoalsAgainst) / 30.0, -0.15, 0.15);
        return Math.Clamp((points / maxPoints) + goalBalanceBonus, 0.05, 0.95);
    }

    private static (double home, double draw, double away) CalculateHistoricalMarketProbability(IEnumerable<HistoricalMatch> history)
    {
        var list = history.ToList();
        if (list.Count == 0)
        {
            return (0.45, 0.25, 0.30);
        }

        var normalized = list.Select(x =>
        {
            var h = 1.0 / (double)x.HomeOdds;
            var d = 1.0 / (double)x.DrawOdds;
            var a = 1.0 / (double)x.AwayOdds;
            var s = h + d + a;
            return (home: h / s, draw: d / s, away: a / s);
        }).ToList();

        return (normalized.Average(x => x.home), normalized.Average(x => x.draw), normalized.Average(x => x.away));
    }
}
