using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    public class AuctionHub : Hub
    {
        private readonly ILogger<AuctionHub> _logger;

        public AuctionHub(ILogger<AuctionHub> logger)
        {
            _logger = logger;
        }
        public async Task JoinAuctionRoom(int auctionId)
        {
            try
            {
                _logger.LogInformation("Connection {ConnectionId} joining auction room {AuctionId}", Context.ConnectionId, auctionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining auction room");
                throw; 
            }
        }

        public async Task LeaveAuctionRoom(int auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId.ToString());
        }
    }
}
