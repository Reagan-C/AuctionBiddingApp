using NotificationService.Infrastructure.Messaging;
using NotificationService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.Services
{
    public class NotificationServiceImp : IHostedService
    {
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly IBidPlacedEventConsumer _bidPlacedEventConsumer;
        private readonly IAuctionEndedEventConsumer _auctionEndedEventConsumer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationServiceImp> _logger;

        public NotificationServiceImp(RabbitMQConnection rabbitMQConnection,
            IBidPlacedEventConsumer bidPlacedEventConsumer,
            IAuctionEndedEventConsumer auctionEndedEventConsumer,
            IConfiguration configuration,
            ILogger<NotificationServiceImp> logger)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _bidPlacedEventConsumer = bidPlacedEventConsumer;
            _auctionEndedEventConsumer = auctionEndedEventConsumer;
            _configuration=configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bidPlacedConsumer = new EventingBasicConsumer(_rabbitMQConnection.Channel);
            var bidPlacedQueueName = _configuration["RabbitMQQueueProperties:BidPlaced"];
            var auctionEndedQueueName = _configuration["RabbitMQQueueProperties:AuctionEnded"];
            
            bidPlacedConsumer.Received += (model, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                _logger.LogInformation("A bid placed message has been received with details; {}", message);
                _bidPlacedEventConsumer.ConsumeEvent(message);
            };
            _rabbitMQConnection.Channel.BasicConsume(bidPlacedQueueName, true, bidPlacedConsumer);

            var auctionEndedConsumer = new EventingBasicConsumer(_rabbitMQConnection.Channel);
            auctionEndedConsumer.Received += (model, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                _logger.LogInformation("An auction ended message has been received with details; {}", message);
                _auctionEndedEventConsumer.ConsumeEvent(message);
            };
            _rabbitMQConnection.Channel.BasicConsume(auctionEndedQueueName, true, auctionEndedConsumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _rabbitMQConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}
