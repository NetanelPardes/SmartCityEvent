using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SensorProducer.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null,string> _producer;
        private readonly string _topicName;

        public KafkaProducerService(string bootstrapServers ,string topicName)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topicName = topicName;
        }

        public async Task<DeliveryResult<Null,string>> Sendasync<T>(string topic , T message)
        {
            string json = JsonSerializer.Serialize(message);
            var kafkaMessge = new Message<Null, string>
            {
                Value = json
            };
            var result = await _producer.ProduceAsync(topic, kafkaMessge);
            return result;
        }
    }
}
