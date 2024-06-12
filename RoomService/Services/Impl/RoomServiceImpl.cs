using Newtonsoft.Json;
using RoomService.Dtos;
using RoomService.Kafka;
using RoomService.Models;
using RoomService.Repositories;
using RoomService.Services.Interface;
using RoomService.Utilities;

namespace RoomService.Services.Impl
{
    public class RoomServiceImpl : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly KafkaProducer _producer;

        public RoomServiceImpl(IRoomRepository roomRepository, KafkaProducer producer)
        {
            _roomRepository=roomRepository;
            _producer=producer;
        }

        public async Task<CreateRoomResponse> CreateRoom(CreateRoomRequest createRoomRequest)
        {
            var room = new Room
            {
                Name = createRoomRequest.Name,
                Description = createRoomRequest.Description
            };

            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            if (createdRoom == null)
                return null;

            return await Task.FromResult(new CreateRoomResponse { Room = createdRoom, IsSuccessful = true });
        }

        public async Task<bool> DeleteRoom(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) return false;

            await _roomRepository.DeleteRoomAsync(roomId);
            return true;
        }

        public async Task<GetRoomResponse> GetRoom(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                return null;

            return await Task.FromResult( new GetRoomResponse { Room = room});
        }

        public async Task<bool> StartAuction(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                return false;

            var auction = new Auction
            {
                RoomId = roomId,
                Status = AuctionStatus.InProgress,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            await _roomRepository.SaveAuctionAsync(auction);
            var message = JsonConvert.SerializeObject(auction);
            await _producer.ProduceMessageAsync(message);
            Console.WriteLine(message);
            return true;
        }
    }
}
