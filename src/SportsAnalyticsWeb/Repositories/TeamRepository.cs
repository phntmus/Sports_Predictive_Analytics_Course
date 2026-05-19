using Microsoft.EntityFrameworkCore;
using SportsAnalyticsWeb.Data;
using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Repositories;

public sealed class TeamRepository : ITeamRepository
{
    private readonly SportsAnalyticsDbContext db;

    public TeamRepository(SportsAnalyticsDbContext db)
    {
        this.db = db;
    }

    public Task<List<Team>> GetAllAsync() => db.Teams.Include(x => x.Statistic).OrderByDescending(x => x.EloRating).ToListAsync();

    public Task<Team?> GetByIdAsync(int id) => db.Teams.Include(x => x.Statistic).FirstOrDefaultAsync(x => x.Id == id);
}
