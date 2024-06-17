using InvoiceService.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceService.Data
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Invoice>(e =>
            {
                e.Property(e => e.WinningBidAmount)
                      .HasColumnType("decimal(18, 2)");
            });
        }
    }
}
