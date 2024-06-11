
using BiddingService.Data;
using BiddingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BiddingService.Repository
{
    public class BidRepository : IBidRepository
    {
        private readonly BiddingDbContext _context;

        public BidRepository(BiddingDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Auction> GetAuctionAsync(int auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            return auction == null ? auction : null;
        }

        public decimal GetHighestBidAmount(int auctionId)
        {
            var highestAmount = _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault()?.Amount ?? 0;
            return  highestAmount;
        }

        public async Task<Bid> GetWinningBid(int auctionId)
        {
            var bid = await _context.Bids
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();
            return bid;
        }

        public async Task SaveAuctionAsync(Auction auction)
        {
            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
        }

        public async Task SaveBidAsync(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }
    }
}
