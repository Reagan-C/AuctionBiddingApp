using InvoiceService.Data;
using InvoiceService.Models;

namespace InvoiceService.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDbContext _context;

        public InvoiceRepository(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            return invoice ?? new Invoice();
        }
    }
}
