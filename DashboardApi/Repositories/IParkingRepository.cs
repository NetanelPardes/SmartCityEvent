using DashboardApi.Models;

namespace DashboardApi.Repositories;

public interface IParkingRepository
{
    Task<IEnumerable<ParkingEvent>> GetAllAsync();

    Task<ParkingEvent?> GetByIdAsync(int id);

    Task<IEnumerable<ParkingEvent>> SearchAsync(string? location);
}