using RoomService.Models;

namespace RoomService.Dtos
{
    public class GetRoomResponse
    {
        public Room Room {  get; set; }
        public string Message { get; set; }
    }
}
