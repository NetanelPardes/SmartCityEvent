using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventConsumerWorker.Data;
using EventConsumerWorker.Services;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Smart City Event Consumer ===\n");
        // ============================================
        // PHASE 1: Setup Configuration and DI
        // ============================================

        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
        var services = new ServiceCollection();

        // Register DbContext
        services.AddDbContext<SmartCityDbContext>(options =>
        options.UseMySql(
        configuration.GetConnectionString("SmartCityDb"),
        ServerVersion.AutoDetect(configuration.GetConnectionString("SmartCityDb"))));

        // Register our processing service
        services.AddScoped<EventProcessingService>();

        var serviceProvider = services.BuildServiceProvider();
        // ============================================
        // PHASE 2: Create Database
        // ============================================

        Console.WriteLine("Creating database...");
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SmartCityDbContext>();
            db.Database.EnsureCreated();
        }
        Console.WriteLine(" Database ready\n");
        // ============================================
        // PHASE 3: Configure Kafka Consumer
        // ============================================

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // We'll commit manually after processing
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        var topics = new[]
 {
 configuration["Kafka:Topics:Traffic"]!,
 configuration["Kafka:Topics:Weather"]!,
 configuration["Kafka:Topics:Parking"]!
 };

        consumer.Subscribe(topics);
        Console.WriteLine($"Subscribed to: {string.Join(", ", topics)}");
        Console.WriteLine("Consuming events... Press Ctrl+C to stop.\n");
        // ============================================
        // PHASE 4: Consume Loop
        // ============================================

        try
        {
            while (true)
            {
                // Wait for a message (timeout after 1 second)
                var result = consumer.Consume(TimeSpan.FromSeconds(1));

                // If no message, continue waiting
                if (result == null || result.Message?.Value == null)
                    continue;
                Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Received from {result.Topic}");
                // Create a new scope for this message
                // This gives us a fresh DbContext
                using (var scope = serviceProvider.CreateScope())
                {
                    var processingService = scope.ServiceProvider
                    .GetRequiredService<EventProcessingService>();

                    // Route to the correct processing method based on topic
                    bool success = result.Topic switch
                    {
                        var t when t == configuration["Kafka:Topics:Traffic"]
                        => await processingService.ProcessTrafficEventAsync(result.Message.Value),
                        var t when t == configuration["Kafka:Topics:Weather"]
                        => await processingService.ProcessWeatherEventAsync(result.Message.Value),
                        var t when t == configuration["Kafka:Topics:Parking"]
                        => await processingService.ProcessParkingEventAsync(result.Message.Value),
                        _ => false
                    };
                    // Commit the offset (tell Kafka we processed this message)
                    if (success)
                    {
                        consumer.Commit(result);
                    }
                    else
                    {
                        Console.WriteLine(" Processing failed, but committing to avoid reprocessing");
                        consumer.Commit(result);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n\nShutting down gracefully...");
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("Consumer closed.");
        }
    }
}