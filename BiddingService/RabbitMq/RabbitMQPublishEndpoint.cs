using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BiddingService.RabbitMq
{
    public class RabbitMQPublishEndpoint : IPublishEndpoint, IDisposable
    {
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public RabbitMQPublishEndpoint(IModel channel, string exchangeName)
        {
            _channel = channel;
            _exchangeName = exchangeName;
        }

        public Task Publish<T>(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(_exchangeName, "", null, body);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
