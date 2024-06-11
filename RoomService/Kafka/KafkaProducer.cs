using Confluent.Kafka;

namespace RoomService.Kafka
{
    public class KafkaProducer : IDisposable
    {
        private const string _topicName = "auction-start-event";
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;

        public KafkaProducer(IConfiguration configuration)
        {
            _configuration = configuration;
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
                await _producer.ProduceAsync(_topicName, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = message
                });
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Failed to deliver message: {e.Error.Reason}");
                throw e;
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}
