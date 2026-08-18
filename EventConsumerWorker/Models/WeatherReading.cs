using System;
using System.Collections.Generic;
using System.Text;

namespace EventConsumerWorker.Models
{
    public class WeatherReading
    {
        public string Location { get; set; } = string.Empty;
        public decimal TemperatureCelsius { get; set; }
        public int Humidity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
