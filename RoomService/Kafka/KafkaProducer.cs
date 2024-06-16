using Confluent.Kafka;

namespace RoomService.Kafka
{
    public class KafkaProducer : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };

            // Validate configuration
            if (string.IsNullOrEmpty(producerConfig.BootstrapServers))
            {
                throw new ArgumentException("Kafka bootstrap servers configuration is missing.");
            }


            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task ProduceMessageAsync(string message)
        {
            try
            {
                var _topicName = _configuration["Kafka:TopicName"];
                await _producer.ProduceAsync(_topicName, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = message
                });
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError($"Failed to deliver message: {e.Error.Reason}");
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}
