using DashboardApi.Models;
using DashboardApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DashboardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrafficController : ControllerBase
{
    private readonly ITrafficRepository _trafficRepository;

    public TrafficController(ITrafficRepository trafficRepository)
    {
        _trafficRepository = trafficRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrafficEvent>>> GetAll()
    {
        var trafficEvents = await _trafficRepository.GetAllAsync();

        return Ok(trafficEvents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrafficEvent>> GetById(int id)
    {
        var trafficEvent = await _trafficRepository.GetByIdAsync(id);

        if (trafficEvent == null)
        {
            return NotFound("Traffic event not found");
        }

        return Ok(trafficEvent);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TrafficEvent>>> Search(
        [FromQuery] string? location)
    {
        var trafficEvents =
            await _trafficRepository.SearchAsync(location);

        return Ok(trafficEvents);
    }
}