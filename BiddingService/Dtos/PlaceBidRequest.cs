using System.ComponentModel.DataAnnotations;

namespace BiddingService.Dtos
{
    public class PlaceBidRequest
    {
        [Required]
        public int AuctionId { get; set; }
        [Required]
        public decimal BidAmount { get; set; }
    }
}
