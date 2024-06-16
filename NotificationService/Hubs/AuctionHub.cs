using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task JoinAuctionRoom(int auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
        }

        public async Task LeaveAuctionRoom(int auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId.ToString());
        }
    }
}
