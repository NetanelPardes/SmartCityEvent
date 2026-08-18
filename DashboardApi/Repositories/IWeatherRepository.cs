using DashboardApi.Models;

namespace DashboardApi.Repositories;

public interface IWeatherRepository
{
    Task<IEnumerable<WeatherEvent>> GetAllAsync();

    Task<WeatherEvent?> GetByIdAsync(int id);

    Task<IEnumerable<WeatherEvent>> SearchAsync(string? location);
}