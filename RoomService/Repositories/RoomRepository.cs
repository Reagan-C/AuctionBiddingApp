using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Dtos;
using RoomService.Models;

namespace RoomService.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DataContext _dataContext;

        public RoomRepository(DataContext dataContext)
        {
            _dataContext=dataContext;
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            await _dataContext.AddAsync(room);
            await _dataContext.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var room =  await _dataContext.Rooms.FindAsync(roomId);
            _dataContext.Rooms.Remove(room);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<Room>> GetAllRoomsAsync()
        {
            return await _dataContext.Rooms.OrderBy(r => r.Id).ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(int roomId)
        {
            return await _dataContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }
    }
}
