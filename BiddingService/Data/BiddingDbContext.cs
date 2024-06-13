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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Bid>(e =>
            {
                e.Property(e => e.Amount)
                      .HasColumnType("decimal(18, 2)"); 
            });
        }
    }
}
