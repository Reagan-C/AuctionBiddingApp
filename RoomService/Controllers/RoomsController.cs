using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomService.Dtos;
using RoomService.Repositories;
using RoomService.Services;

namespace RoomService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize (Roles = "Admin")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomService roomService, IRoomRepository roomRepository)
        {
            _roomService=roomService;
            _roomRepository=roomRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRoom = await _roomRepository.GetRoomByName(request.Name);
            if (existingRoom != null)
                return BadRequest($"Room with name {request.Name} already exists");

            var roomResponse = await _roomService.CreateRoom(request);
            if (roomResponse == null)
                return BadRequest("Error encountered while creating room");

            return Ok(roomResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoom(int roomId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomResponse = await _roomService.GetRoom(roomId);
            if (roomResponse == null)
                return NotFound($"Room with Id {roomId} not found");

            return Ok(roomResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool isSuccessful = await _roomService.DeleteRoom(roomId);
            if (!isSuccessful)
                return StatusCode(500, "Delete operation failed");

            return NoContent();
        }

        [HttpPost("/start-auction")]
        public async Task<IActionResult> StartAuction(StartAuctionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _roomRepository.GetRoomByIdAsync(request.RoomId);
            if (room == null)
                return BadRequest($"Room with id {request.RoomId} not found");

            var response = await _roomService.StartAuction(request);
            if (!response)
            {
                return BadRequest($"Auction already in progress for room {request.RoomId}");
            }

            return Ok("Auction started");
        }
    }
}
