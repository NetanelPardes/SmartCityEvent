using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;

namespace SensorProducer.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(string bootstrapServers)
        {
            _bootstrapServers = bootstrapServers;

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task EnsureTopicExistsAsync(
            string topicName,
            int numPartitions = 1,
            short replicationFactor = 1)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _bootstrapServers
            };

            using var adminClient =
                new AdminClientBuilder(config).Build();

            try
            {
                await adminClient.CreateTopicsAsync(
                    new[]
                    {
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = numPartitions,
                            ReplicationFactor = replicationFactor
                        }
                    });

                Console.WriteLine(
                    $"Topic '{topicName}' created successfully.");
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results[0].Error.Code ==
                    ErrorCode.TopicAlreadyExists)
                {
                    Console.WriteLine(
                        $"Topic '{topicName}' already exists.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<DeliveryResult<Null, string>>
            SendAsync<T>(string topic, T message)
        {
            string json =
                JsonSerializer.Serialize(message);

            var kafkaMessage =
                new Message<Null, string>
                {
                    Value = json
                };

            var result =
                await _producer.ProduceAsync(
                    topic,
                    kafkaMessage);

            Console.WriteLine(
                $"Sent message to {result.TopicPartitionOffset}");

            return result;
        }
    }
}