using DashboardApi.Data;
using DashboardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardApi.Repositories;

public class TrafficRepository : ITrafficRepository
{
    private readonly SmartCityDbContext _dbContext;

    public TrafficRepository(SmartCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TrafficEvent>> GetAllAsync()
    {
        return await _dbContext.TrafficEvents
            .ToListAsync();
    }

    public async Task<TrafficEvent?> GetByIdAsync(int id)
    {
        return await _dbContext.TrafficEvents
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TrafficEvent>> SearchAsync(string? location)
    {
        var query = _dbContext.TrafficEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(t => t.Location.Contains(location));
        }

        return await query.ToListAsync();
    }
}