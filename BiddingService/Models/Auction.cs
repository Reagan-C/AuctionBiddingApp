using BiddingService.Utilities;
using Newtonsoft.Json;

namespace BiddingService.Models
{
    public class Auction
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionStatus Status { get; set; } = AuctionStatus.InProgress;
        [JsonIgnore]
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}
