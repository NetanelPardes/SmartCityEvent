using System;
using System.Collections.Generic;
using System.Text;

namespace SensorProducer.Models
{
    public class ParkingReading
    {
        public string Location { get; set; } = string.Empty;
        public int AvailableSpots { get; set; }
        public int TotalSpots { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
