using DashboardApi.Data;
using DashboardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardApi.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly SmartCityDbContext _dbContext;

    public WeatherRepository(SmartCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<WeatherEvent>> GetAllAsync()
    {
        return await _dbContext.WeatherEvents
            .ToListAsync();
    }

    public async Task<WeatherEvent?> GetByIdAsync(int id)
    {
        return await _dbContext.WeatherEvents
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<WeatherEvent>> SearchAsync(string? location)
    {
        var query = _dbContext.WeatherEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(w => w.Location.Contains(location));
        }

        return await query.ToListAsync();
    }
}