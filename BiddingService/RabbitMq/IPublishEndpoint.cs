namespace BiddingService.RabbitMq
{
    public interface IPublishEndpoint
    {
        Task Publish<T>(T message, string queueName);
    }
}
