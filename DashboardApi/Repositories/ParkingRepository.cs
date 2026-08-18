using DashboardApi.Data;
using DashboardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardApi.Repositories;

public class ParkingRepository : IParkingRepository
{
    private readonly SmartCityDbContext _dbContext;

    public ParkingRepository(SmartCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ParkingEvent>> GetAllAsync()
    {
        return await _dbContext.ParkingEvents
            .ToListAsync();
    }

    public async Task<ParkingEvent?> GetByIdAsync(int id)
    {
        return await _dbContext.ParkingEvents
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ParkingEvent>> SearchAsync(string? location)
    {
        var query = _dbContext.ParkingEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(p => p.Location.Contains(location));
        }

        return await query.ToListAsync();
    }
}