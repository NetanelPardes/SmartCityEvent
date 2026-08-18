using Microsoft.Extensions.Configuration;
using SensorProducer.Services;

namespace SmartCityEventProcessingSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(
                    Directory.GetCurrentDirectory())
                .AddJsonFile(
                    "appsettings.json",
                    optional: false)
                .Build();

        string bootstrapServers =
            configuration["Kafka:BootstrapServers"]!;

        string trafficTopic =
            configuration["Kafka:Topics:Traffic"]!;

        string weatherTopic =
            configuration["Kafka:Topics:Weather"]!;

        string parkingTopic =
            configuration["Kafka:Topics:Parking"]!;

        var dataLoader =
            new DataLoaderService();

        var parking =
            dataLoader.LoadParkingData(
                "Data/parking-data.json");

        var traffic =
            dataLoader.LoadTrafficData(
                "Data/traffic-data.json");

        var weather =
            dataLoader.LoadWeatherData(
                "Data/weather-data.json");

        var producer =
            new KafkaProducerService(
                bootstrapServers);

        // ==========================================
        // Ensure topics exist
        // ==========================================

        await producer.EnsureTopicExistsAsync(
            trafficTopic);

        await producer.EnsureTopicExistsAsync(
            weatherTopic);

        await producer.EnsureTopicExistsAsync(
            parkingTopic);

        // ==========================================
        // Send parking events
        // ==========================================

        foreach (var item in parking)
        {
            await producer.SendAsync(
                parkingTopic,
                item);
        }

        // ==========================================
        // Send traffic events
        // ==========================================

        foreach (var item in traffic)
        {
            await producer.SendAsync(
                trafficTopic,
                item);
        }

        // ==========================================
        // Send weather events
        // ==========================================

        foreach (var item in weather)
        {
            await producer.SendAsync(
                weatherTopic,
                item);
        }

        Console.WriteLine(
            "\nAll events sent successfully.");
    }
}