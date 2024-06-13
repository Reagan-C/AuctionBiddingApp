using BiddingService.Dtos;
using BiddingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly BidService _bidService;

        public BidController(BidService bidService)
        {
            _bidService = bidService;
        }

        [HttpPost("{userId}/bids")]
        public async Task<IActionResult> PlaceBid(string userId, [FromBody] PlaceBidRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bidService.PlaceBid(userId, request);
            return Ok("Bid placed");
        }

        [HttpPost("{auctionId}/end")]
        public async Task<IActionResult> EndAuction(int auctionId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bidService.EndAuction(auctionId);
                return Ok("Auction ended");
        }
    }
}
