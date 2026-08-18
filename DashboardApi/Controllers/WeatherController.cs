using DashboardApi.Models;
using DashboardApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DashboardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepository;

    public WeatherController(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherEvent>>> GetAll()
    {
        var weatherEvents =
            await _weatherRepository.GetAllAsync();

        return Ok(weatherEvents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeatherEvent>> GetById(int id)
    {
        var weatherEvent =
            await _weatherRepository.GetByIdAsync(id);

        if (weatherEvent == null)
        {
            return NotFound("Weather event not found");
        }

        return Ok(weatherEvent);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<WeatherEvent>>> Search(
        [FromQuery] string? location)
    {
        var weatherEvents =
            await _weatherRepository.SearchAsync(location);

        return Ok(weatherEvents);
    }
}