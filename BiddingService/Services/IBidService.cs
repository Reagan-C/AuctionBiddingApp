using BiddingService.Dtos;

namespace BiddingService.Services
{
    public interface IBidService
    {
        Task PlaceBid(string userId, PlaceBidRequest request);
        Task EndAuction(int auctionId);
    }
}
