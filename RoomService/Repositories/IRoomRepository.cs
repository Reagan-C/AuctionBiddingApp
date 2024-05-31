using RoomService.Dtos;
using RoomService.Models;

namespace RoomService.Repositories
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByIdAsync(int roomId);
        Task<ICollection<Room>> GetAllRoomsAsync();
        Task <Room> CreateRoomAsync(Room request);
        Task<bool> DeleteRoomAsync(int roomId);
    }
}
