using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardApi.Models;

public class ParkingEvent
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public int AvailableSpots { get; set; }
    public int TotalSpots { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime ProcessedAt { get; set; }
}