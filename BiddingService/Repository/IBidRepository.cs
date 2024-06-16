using BiddingService.Models;

namespace BiddingService.Repository
{
    public interface IBidRepository
    {
        Task SaveAuctionAsync(Auction auction);
        Task UpdateAuctionAsync(Auction auction);
        Task<Auction> GetAuctionAsync(int auctionId);
        Task<Bid> GetWinningBid(int auctionId);
        decimal GetHighestBidAmount(int auctionId);
        Task SaveBidAsync(Bid bid);
    }
}
