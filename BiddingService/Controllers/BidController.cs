using BiddingService.Dtos;
using BiddingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bidService;

        public BidController(IBidService bidService)
        {
            _bidService = bidService;
        }

        [HttpPost("{userId}/bids")]
        public async Task<IActionResult> PlaceBid(string userId, [FromBody] PlaceBidRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bidService.PlaceBid(userId, request);
            if (!result.Success) 
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("{auctionId}/end")]
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
