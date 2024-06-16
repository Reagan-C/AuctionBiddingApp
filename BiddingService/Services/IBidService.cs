using BiddingService.Dtos;

namespace BiddingService.Services
{
    public interface IBidService
    {
        Task<BidResult> PlaceBid(string userId, PlaceBidRequest request);
        Task<EndAuctionResult> EndAuction(int auctionId);
    }
}
