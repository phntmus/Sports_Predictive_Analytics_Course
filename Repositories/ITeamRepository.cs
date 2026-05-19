using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Repositories;

public interface ITeamRepository
{
    Task<List<Team>> GetAllAsync();

    Task<Team?> GetByIdAsync(int id);
}
