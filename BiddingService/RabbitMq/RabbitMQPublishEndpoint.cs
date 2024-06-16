using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BiddingService.RabbitMq
{
    public class RabbitMQPublishEndpoint : IPublishEndpoint
    {
        private readonly IConfiguration _configuration;
        public RabbitMQPublishEndpoint(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task Publish<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMq:UserName"],
                Password = _configuration["RabbitMq:Password"],
                VirtualHost = _configuration["RabbitMq:VirtualHost"]
            };
            
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queueName, durable: true, exclusive: false);

            var jsonString = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish("", queueName, body: body);
            return Task.CompletedTask;
        }
    }
}
