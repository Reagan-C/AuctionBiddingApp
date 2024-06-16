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
    }
}
