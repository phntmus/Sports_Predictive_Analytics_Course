using Microsoft.EntityFrameworkCore;
using SportsAnalyticsWeb.Data;
using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Repositories;

public sealed class MatchRepository : IMatchRepository
{
    private readonly SportsAnalyticsDbContext db;

    public MatchRepository(SportsAnalyticsDbContext db)
    {
        this.db = db;
    }
    public Task<List<Match>> GetUpcomingAsync() => db.Matches
        .Include(x => x.HomeTeam).ThenInclude(x => x!.Statistic)
        .Include(x => x.AwayTeam).ThenInclude(x => x!.Statistic)
        .Include(x => x.Prediction)
        .OrderBy(x => x.StartTime)
        .ToListAsync();

    public Task<Match?> GetByIdAsync(int id) => db.Matches
        .Include(x => x.HomeTeam).ThenInclude(x => x!.Statistic)
        .Include(x => x.AwayTeam).ThenInclude(x => x!.Statistic)
        .Include(x => x.Prediction)
        .FirstOrDefaultAsync(x => x.Id == id);
}
