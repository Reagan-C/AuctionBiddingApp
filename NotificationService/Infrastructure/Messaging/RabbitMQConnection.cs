
using NotificationService.Configurations;
using RabbitMQ.Client;

namespace NotificationService.Infrastructure.Messaging
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConnection(IConfiguration configuration)
        {
            var rabbitMQSettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.HostName,
                UserName = rabbitMQSettings.UserName,
                Password = rabbitMQSettings.Password,
                VirtualHost = rabbitMQSettings.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare queues on startup
            var bidPlacedQueue = configuration["RabbitMQQueueProperties:BidPlaced"];
            var auctionEndedQueue = configuration["RabbitMQQueueProperties:AuctionEnded"];
            var invoiceDetailsQueue = configuration["RabbitMQQueueProperties:GenerateInvoiceRequest"];

            _channel.QueueDeclare(queue: bidPlacedQueue,
                                  durable: true,
                                  exclusive: false);

            _channel.QueueDeclare(queue: auctionEndedQueue,
                                  durable: true,
                                  exclusive: false);

            _channel.QueueDeclare(queue: invoiceDetailsQueue,
                                  durable: true,
                                  exclusive: false);
        }

        public IModel Channel => _channel;

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
