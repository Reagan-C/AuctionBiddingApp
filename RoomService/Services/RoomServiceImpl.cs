using Newtonsoft.Json;
using RoomService.Dtos;
using RoomService.Kafka;
using RoomService.Models;
using RoomService.Repositories;

namespace RoomService.Services
{
    public class RoomServiceImpl : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly KafkaProducer _producer;
        private readonly ILogger<RoomServiceImpl> _logger;

        public RoomServiceImpl(IRoomRepository roomRepository, KafkaProducer producer, ILogger<RoomServiceImpl> logger)
        {
            _roomRepository = roomRepository;
            _producer = producer;
            _logger=logger;
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
            _logger.LogInformation($"Room created with ID {createdRoom.Id}");
            return await Task.FromResult(new CreateRoomResponse { Room = createdRoom, IsSuccessful = true });
        }

        public async Task<bool> DeleteRoom(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) 
                return false;

            await _roomRepository.DeleteRoomAsync(roomId);
            _logger.LogInformation("Deletion successful");
            return true;
        }

        public async Task<GetRoomResponse> GetRoom(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                _logger.LogWarning("Invalid Id");
                return null;
            }
            return await Task.FromResult(new GetRoomResponse { Room = room });
        }

        public async Task<bool> StartAuction(StartAuctionRequest request)
        {
            var checkAuction = await _roomRepository.GetAuctionByRoomId(request.RoomId);
            if (checkAuction != null)
                return false;

            var auction = new Auction
            {
                RoomId = request.RoomId,
                ItemName = request.ItemName,
                StartTime = DateTime.UtcNow
            };

            await _roomRepository.SaveAuctionAsync(auction);
            _logger.LogInformation("Auction saved");
            var message = JsonConvert.SerializeObject(auction);
            await _producer.ProduceMessageAsync(message);
            _logger.LogInformation($"Auction started with details {auction}", auction);
            return true;
        }
    }
}
