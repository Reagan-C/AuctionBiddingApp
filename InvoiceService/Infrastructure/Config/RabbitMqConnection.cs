using RabbitMQ.Client;

namespace InvoiceService.Infrastructure.Config
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                VirtualHost = configuration["RabbitMQ:VirtualHost"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //declare queues on startup
            var invoiceDetailsQueue = configuration["RabbitMQ:InvoiceQueueName"];
            var paymentRequestQueue = configuration["RabbitMQ:PaymentRequestQueueName"];

            _channel.QueueDeclare(queue: invoiceDetailsQueue,
                                  durable: true,
                                  exclusive: false);

            _channel.QueueDeclare(queue: paymentRequestQueue,
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
