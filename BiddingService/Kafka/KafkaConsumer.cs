
using BiddingService.Models;
using BiddingService.Repository;
using BiddingService.Utilities;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace BiddingService.Kafka
{
    public class KafkaConsumer : IHostedService
    {
        private readonly IConfiguration _configuration; 
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumer> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public KafkaConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<KafkaConsumer> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task.Run(() => ConsumeMessages(_cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        private async Task ConsumeMessages(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_configuration["Kafka:TopicName"]);
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bidRepository = scope.ServiceProvider.GetRequiredService<IBidRepository>();
                    await ProcessAuctionStartedAsync(consumeResult.Message.Value, bidRepository);
                }
                _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at:" +
                   $" '{consumeResult.TopicPartitionOffset}'.");
            }

            consumer.Close();
        }

        private async Task ProcessAuctionStartedAsync(string message, IBidRepository repository)
        {
            var auction = JsonConvert.DeserializeObject<Auction>(message);
            if (auction != null)
            {
                auction.Status = AuctionStatus.InProgress;
                auction.Id = 0;
                await repository.SaveAuctionAsync(auction);
                _logger.LogInformation("Message consumed and saved to auctions");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
