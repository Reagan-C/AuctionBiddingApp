using BiddingService.Utilities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BiddingService.Models
{
    public class Auction
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionStatus Status { get; set; } = AuctionStatus.NotStarted;
        [JsonIgnore]
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}
