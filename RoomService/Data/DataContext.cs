using Microsoft.EntityFrameworkCore;
using RoomService.Models;

namespace RoomService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>options) : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Auction> Auctions { get; set; }
    }
}
