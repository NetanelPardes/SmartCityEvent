using DashboardApi.Models;
using DashboardApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DashboardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingRepository _parkingRepository;

    public ParkingController(IParkingRepository parkingRepository)
    {
        _parkingRepository = parkingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParkingEvent>>> GetAll()
    {
        var parkingEvents =
            await _parkingRepository.GetAllAsync();

        return Ok(parkingEvents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingEvent>> GetById(int id)
    {
        var parkingEvent =
            await _parkingRepository.GetByIdAsync(id);

        if (parkingEvent == null)
        {
            return NotFound("Parking event not found");
        }

        return Ok(parkingEvent);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ParkingEvent>>> Search(
        [FromQuery] string? location)
    {
        var parkingEvents =
            await _parkingRepository.SearchAsync(location);

        return Ok(parkingEvents);
    }
}