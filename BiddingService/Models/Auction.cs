using BiddingService.Utilities;

namespace BiddingService.Models
{
    public class Auction
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionStatus Status { get; set; }
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}
