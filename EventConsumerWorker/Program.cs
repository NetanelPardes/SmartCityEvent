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
        var connectionString =
            configuration.GetConnectionString("SmartCityDb");

        services.AddDbContext<SmartCityDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
        );

        // Register processing service
        services.AddScoped<EventProcessingService>();

        var serviceProvider = services.BuildServiceProvider();

        // ============================================
        // PHASE 2: Create Database
        // ============================================

        Console.WriteLine("Creating database...");

        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<SmartCityDbContext>();

            db.Database.EnsureCreated();
        }

        Console.WriteLine("Database ready\n");

        // ============================================
        // PHASE 3: Configure Kafka Consumer
        // ============================================

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer =
            new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        // Read topic names once
        var trafficTopic =
            configuration["Kafka:Topics:traffic"]!;

        var weatherTopic =
            configuration["Kafka:Topics:weather"]!;

        var parkingTopic =
            configuration["Kafka:Topics:parking"]!;

        var topics = new[]
        {
            trafficTopic,
            weatherTopic,
            parkingTopic
        };

        consumer.Subscribe(topics);

        Console.WriteLine(
            $"Subscribed to: {string.Join(", ", topics)}"
        );

        Console.WriteLine(
            "Consuming events... Press Ctrl+C to stop.\n"
        );

        // ============================================
        // PHASE 4: Consume Loop
        // ============================================

        try
        {
            while (true)
            {
                // Wait for message
                var result =
                    consumer.Consume(TimeSpan.FromSeconds(1));

                // No message received
                if (result == null ||
                    result.Message?.Value == null)
                {
                    continue;
                }

                Console.WriteLine(
                    $"\n[{DateTime.Now:HH:mm:ss}] " +
                    $"Received from {result.Topic}"
                );

                // Create a new DI scope for every message
                using (var scope = serviceProvider.CreateScope())
                {
                    var processingService =
                        scope.ServiceProvider
                            .GetRequiredService<EventProcessingService>();

                    bool success = result.Topic switch
                    {
                        var topic when topic == trafficTopic
                            => await processingService
                                .ProcessTrafficEventAsync(
                                    result.Message.Value
                                ),

                        var topic when topic == weatherTopic
                            => await processingService
                                .ProcessWeatherEventAsync(
                                    result.Message.Value
                                ),

                        var topic when topic == parkingTopic
                            => await processingService
                                .ProcessParkingEventAsync(
                                    result.Message.Value
                                ),

                        _ => false
                    };

                    // Commit offset after processing
                    if (success)
                    {
                        Console.WriteLine(
                            "Event processed successfully."
                        );

                        consumer.Commit(result);
                    }
                    else
                    {
                        Console.WriteLine(
                            "Processing failed, " +
                            "committing to avoid reprocessing."
                        );

                        consumer.Commit(result);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(
                "\nShutting down gracefully..."
            );
        }
        catch (ConsumeException ex)
        {
            Console.WriteLine(
                $"Kafka consume error: {ex.Error.Reason}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error: {ex.Message}"
            );
        }
        finally
        {
            consumer.Close();

            Console.WriteLine("Consumer closed.");

            serviceProvider.Dispose();
        }
    }
}