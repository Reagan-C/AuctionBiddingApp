using BiddingService.Dtos;
using BiddingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bidService;

        public BidController(IBidService bidService)
        {
            _bidService = bidService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bidService.PlaceBid(userId, request);
            if (!result.Success) 
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("{auctionId}/end")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndAuction(int auctionId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bidService.EndAuction(auctionId);
            if (!result.Success) 
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
