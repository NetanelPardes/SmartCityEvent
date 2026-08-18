using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardApi.Models;

public class TrafficEvent
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime ProcessedAt { get; set; }
}
