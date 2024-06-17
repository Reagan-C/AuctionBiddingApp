using Newtonsoft.Json;

namespace RoomService.Models
{
    public class Auction
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public Room Room { get; set; }
    }
}
