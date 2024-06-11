
using BiddingService.Data;
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
        private readonly BiddingDbContext _dbContext;
        private readonly IBidRepository _bidRepository;

        public KafkaConsumer(IConfiguration configuration, BiddingDbContext dbContext, IBidRepository bidRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _bidRepository=bidRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_configuration["Kafka:TopicName"]);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    ProcessAuctionStarted(consumeResult.Message.Value);
                    Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at:" +
                        $" '{consumeResult.TopicPartitionOffset}'.");
                }

                consumer.Close();
            }

            return Task.CompletedTask;
        }

        private void ProcessAuctionStarted(string message)
        {
            var auction = JsonConvert.DeserializeObject<Auction>(message);

            // Validate and save the auction to the database
            if (auction != null)
            {
                auction.Status = AuctionStatus.InProgress;
                _bidRepository.SaveAuctionAsync(auction);
                Console.WriteLine($"Auction {auction.Id} started successfully.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
