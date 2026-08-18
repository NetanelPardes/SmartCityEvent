using System;
using System.Collections.Generic;
using System.Text;

namespace SensorProducer.Models
{
    public class TrafficReading
    {
        public string Location { get; set; } = string.Empty;
        public int VehicleCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
