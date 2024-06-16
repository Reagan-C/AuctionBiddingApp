using RoomService.Dtos;

namespace RoomService.Services
{
    public interface IRoomService
    {
        Task<CreateRoomResponse> CreateRoom(CreateRoomRequest createRoomRequest);
        Task<GetRoomResponse> GetRoom(int roomId);
        Task<bool> DeleteRoom(int roomId);
        Task<bool> StartAuction(StartAuctionRequest request);
    }
}
