using Newtonsoft.Json;

namespace RoomService.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Auction> Auctions { get; set; } = new HashSet<Auction>();
    }
}
