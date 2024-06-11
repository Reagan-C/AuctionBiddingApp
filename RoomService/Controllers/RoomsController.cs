using Microsoft.AspNetCore.Mvc;
using RoomService.Dtos;
using RoomService.Services.Interface;

namespace RoomService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService=roomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        [HttpPost("{roomId}/start-auction")]
        public async Task<IActionResult> StartAuction(int roomId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _roomService.StartAuction(roomId);
            if (!response)
            {
                return BadRequest($"Room with ID {roomId} not found");
            }

            return Ok("Auction started");
        }
    }
}
