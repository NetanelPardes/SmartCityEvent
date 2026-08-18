using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SensorProducer.Models;
using SensorProducer.Services;

namespace SensorProducer.Services
{
    public class DataLoaderService
    {
        public List<ParkingReading> LoadParkingData(string filePath)
        {
            string parking = File.ReadAllText(filePath);//"Data/parking-data.json"
            List<ParkingReading>? parkingReadings = JsonSerializer.Deserialize<List<ParkingReading>>(parking);
            return parkingReadings ?? new List<ParkingReading>();
        }

        public List<TrafficReading> LoadTrafficData(string filePath)
        {
            string traffic = File.ReadAllText(filePath);//"Data/traffic-data.json"
            List<TrafficReading>? trafficReadings = JsonSerializer.Deserialize<List<TrafficReading>>(traffic);
            return trafficReadings ?? new List<TrafficReading>();
        }

        public List<WeatherReading> LoadWeatherData(string filePath)
        {
            string weather = File.ReadAllText(filePath);//"Data/parking-data.json"
            List<WeatherReading>? weatherReadings = JsonSerializer.Deserialize<List<WeatherReading>>(weather);
            return weatherReadings ?? new List<WeatherReading>();
        }
    }
}
