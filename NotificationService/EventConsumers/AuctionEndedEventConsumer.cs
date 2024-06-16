using Microsoft.AspNetCore.SignalR;
using NotificationService.Dtos;
using NotificationService.Events;
using NotificationService.Hubs;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace NotificationService.EventConsumers
{
    public class AuctionEndedEventConsumer : IAuctionEndedEventConsumer
    {
        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly ILogger<AuctionEndedEventConsumer> _logger;
        private readonly IConfiguration _configuration;

        public AuctionEndedEventConsumer(IHubContext<AuctionHub> hubContext,
            ILogger<AuctionEndedEventConsumer> logger, IConfiguration configuration, RabbitMQConnection rabbitMQConnection)
        {
            _hubContext = hubContext;
            _logger = logger;
            _configuration = configuration;
            _rabbitMQConnection = rabbitMQConnection;
        }

        public async void ConsumeEvent(string message)
        {
            var auctionEndedEvent = JsonSerializer.Deserialize<AuctionEndedEvent>(message);
            await _hubContext.Clients.Group(auctionEndedEvent.AuctionId.ToString())
                .SendAsync("AuctionEnded", auctionEndedEvent);
            _logger.LogInformation("Auction info sent to the chatbox");
            var invoiceRequest = new InvoiceRequest
            {
                AuctionId = auctionEndedEvent.AuctionId,
                BidderId = auctionEndedEvent.UserId,
                BidItemName = auctionEndedEvent.ItemName,
                WinningBidAmount = auctionEndedEvent.Amount
            };

            _logger.LogInformation("Publishing invoice request to invoice service...");

            var invoiceDetailsQueue = _configuration["RabbitMQQueueProperties:GenerateInvoiceRequest"];
            var invoiceRequestJson = JsonSerializer.Serialize(invoiceRequest);
            var body = Encoding.UTF8.GetBytes(invoiceRequestJson);

            _rabbitMQConnection.Channel.BasicPublish("", invoiceDetailsQueue, body:body);

            _logger.LogInformation("Invoice published successfully.");
           
        }
    }
}


