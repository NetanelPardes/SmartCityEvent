using Microsoft.Extensions.Configuration;
using SensorProducer.Services;

namespace SmartCityEventProcessingSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false).Build();
        
        string bootstrapServers = configuration["Kafka:BootstrapServers"]!;
        string trafficTopic = configuration["Kafka:Topics:Traffic"]!;
        string weatherTopic =configuration["Kafka:Topics:Weather"]!;
        string parkingTopic = configuration["Kafka:Topics:Parking"]!;

        var ser = new DataLoaderService();

        var Parking = ser.LoadParkingData("Data/parking-data.json");
        var Traffic = ser.LoadTrafficData("Data/traffic-data.json");
        var Weather = ser.LoadWeatherData("Data/weather-data.json");

        var trafficProducer = new KafkaProducerService(bootstrapServers, trafficTopic);
        var weatherProducer = new KafkaProducerService(bootstrapServers, weatherTopic);
        var parkingProducer = new KafkaProducerService(bootstrapServers, parkingTopic);

        foreach (var item in Parking)
        {
            await trafficProducer.Sendasync(parkingTopic, item);
        }
        foreach (var item in Traffic)
        {
            await trafficProducer.Sendasync(trafficTopic, item);
        }
        foreach (var item in Weather)
        {
            await weatherProducer.Sendasync(weatherTopic, item);
        }

        Console.WriteLine("print a success message");

    }
}
