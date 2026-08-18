using System.Text.Json;
using EventConsumerWorker.Data;
using EventConsumerWorker.Models;

namespace EventConsumerWorker.Services;

public class EventProcessingService
{
    private readonly SmartCityDbContext _dbContext;

    public EventProcessingService(SmartCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ProcessTrafficEventAsync(string jsonMessage)
    {
        try
        {
            // Step 1: Deserialize
            var reading = JsonSerializer.Deserialize<TrafficReading>(jsonMessage);

            if (reading == null)
            {
                Console.WriteLine("Failed to deserialize traffic event");
                return false;
            }

            // Step 2: Validate
            if (!ValidateTraffic(reading))
            {
                Console.WriteLine($"Invalid traffic event: {jsonMessage}");
                return false;
            }

            // Step 3: Transform to database model
            var trafficEvent = new TrafficEvent
            {
                Location = reading.Location,
                VehicleCount = reading.VehicleCount,
                Timestamp = reading.Timestamp,
                ProcessedAt = DateTime.UtcNow
            };

            // Step 4: Save to database
            _dbContext.TrafficEvents.Add(trafficEvent);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine(
                $"Saved traffic event: {reading.Location} - {reading.VehicleCount} vehicles"
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing traffic event: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessWeatherEventAsync(string jsonMessage)
    {
        try
        {
            // Step 1: Deserialize
            var reading = JsonSerializer.Deserialize<WeatherReading>(jsonMessage);

            if (reading == null)
            {
                Console.WriteLine("Failed to deserialize weather event");
                return false;
            }

            // Step 2: Validate
            if (!ValidateWeather(reading))
            {
                Console.WriteLine($"Invalid weather event: {jsonMessage}");
                return false;
            }

            // Step 3: Transform to database model
            var weatherEvent = new WeatherEvent
            {
                Location = reading.Location,
                TemperatureCelsius = reading.TemperatureCelsius,
                Humidity = reading.Humidity,
                Timestamp = reading.Timestamp,
                ProcessedAt = DateTime.UtcNow
            };

            // Step 4: Save to database
            _dbContext.WeatherEvents.Add(weatherEvent);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine(
                $"Saved weather event: {reading.Location} - {reading.TemperatureCelsius}°C"
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing weather event: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessParkingEventAsync(string jsonMessage)
    {
        try
        {
            // Step 1: Deserialize
            var reading = JsonSerializer.Deserialize<ParkingReading>(jsonMessage);

            if (reading == null)
            {
                Console.WriteLine("Failed to deserialize parking event");
                return false;
            }

            // Step 2: Validate
            if (!ValidateParking(reading))
            {
                Console.WriteLine($"Invalid parking event: {jsonMessage}");
                return false;
            }

            // Step 3: Transform to database model
            var parkingEvent = new ParkingEvent
            {
                Location = reading.Location,
                AvailableSpots = reading.AvailableSpots,
                TotalSpots = reading.TotalSpots,
                Timestamp = reading.Timestamp,
                ProcessedAt = DateTime.UtcNow
            };

            // Step 4: Save to database
            _dbContext.ParkingEvents.Add(parkingEvent);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine(
                $"Saved parking event: {reading.Location} - {reading.AvailableSpots}/{reading.TotalSpots}"
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing parking event: {ex.Message}");
            return false;
        }
    }

    private bool ValidateTraffic(TrafficReading reading)
    {
        if (string.IsNullOrWhiteSpace(reading.Location))
            return false;

        if (reading.VehicleCount < 0 || reading.VehicleCount > 1000)
            return false;

        if (reading.Timestamp > DateTime.UtcNow.AddMinutes(5))
            return false;

        return true;
    }

    private bool ValidateWeather(WeatherReading reading)
    {
        if (string.IsNullOrWhiteSpace(reading.Location))
            return false;

        if (reading.TemperatureCelsius < -50 ||
            reading.TemperatureCelsius > 60)
            return false;

        if (reading.Humidity < 0 || reading.Humidity > 100)
            return false;

        if (reading.Timestamp > DateTime.UtcNow.AddMinutes(5))
            return false;

        return true;
    }

    private bool ValidateParking(ParkingReading reading)
    {
        if (string.IsNullOrWhiteSpace(reading.Location))
            return false;

        if (reading.AvailableSpots < 0 || reading.TotalSpots <= 0)
            return false;

        if (reading.AvailableSpots > reading.TotalSpots)
            return false;

        if (reading.Timestamp > DateTime.UtcNow.AddMinutes(5))
            return false;

        return true;
    }
}