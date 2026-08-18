using DashboardApi.Models;

namespace DashboardApi.Repositories;

public interface ITrafficRepository
{
    Task<IEnumerable<TrafficEvent>> GetAllAsync();

    Task<TrafficEvent?> GetByIdAsync(int id);

    Task<IEnumerable<TrafficEvent>> SearchAsync(string? location);
}