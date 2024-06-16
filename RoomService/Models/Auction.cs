using Newtonsoft.Json;
using RoomService.Utilities;

namespace RoomService.Models
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
        public Room Room { get; set; }
    }
}
