using BiddingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BiddingService.Data
{
    public class BiddingDbContext : DbContext
    {
        public BiddingDbContext(DbContextOptions<BiddingDbContext> options) : base(options)
        {
            
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }
}
