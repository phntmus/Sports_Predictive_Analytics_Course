using SportsAnalyticsWeb.Models;

namespace SportsAnalyticsWeb.Repositories;

public interface IMatchRepository
{
    Task<List<Match>> GetUpcomingAsync();

    Task<Match?> GetByIdAsync(int id);
}
