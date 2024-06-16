using Microsoft.AspNetCore.SignalR;
using NotificationService.Events;
using NotificationService.Hubs;
using NotificationService.Interfaces;
using System.Text.Json;

namespace NotificationService.EventConsumers
{
    public class BidPlacedEventConsumer : IBidPlacedEventConsumer
    {
        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly ILogger<BidPlacedEventConsumer> _logger;

        public BidPlacedEventConsumer(IHubContext<AuctionHub> hubContext, ILogger<BidPlacedEventConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public void ConsumeEvent(string message)
        {
            var bidPlacedEvent = JsonSerializer.Deserialize<BidPlacedEvent>(message);
            _hubContext.Clients.Group(bidPlacedEvent.AuctionId.ToString())
                .SendAsync("BidUpdated", bidPlacedEvent);
            _logger.LogInformation("Bid placement details: {}", bidPlacedEvent);
        }
    }
}
