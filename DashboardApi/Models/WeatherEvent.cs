using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardApi.Models;

public class WeatherEvent
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal TemperatureCelsius { get; set; }
    public int Humidity { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime ProcessedAt { get; set; }
}
