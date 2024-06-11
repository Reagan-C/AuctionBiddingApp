using RoomService.Dtos;

namespace RoomService.Services.Interface
{
    public interface IRoomService
    {
        Task<CreateRoomResponse> CreateRoom(CreateRoomRequest createRoomRequest);
        Task<bool> StartAuction(int roomId);
        Task<GetRoomResponse> GetRoom(int roomId);
        Task<bool> DeleteRoom(int roomId);
    }
}
