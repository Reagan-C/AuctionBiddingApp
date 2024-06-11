using RoomService.Models;

namespace RoomService.Dtos
{
    public class CreateRoomResponse
    {
        public Room? Room {  get; set; }
        public bool IsSuccessful { get; set; }
    }
}
