using InvoiceService.Data;
using InvoiceService.Models;
using InvoiceService.Utilities;

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
            return invoice;
        }

        public Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var invoice = _context.Invoices.Find(invoiceId);
            if (invoice == null)
            {
                return Task.CompletedTask;
            }

            invoice.Status = status;
            _context.Invoices.Update(invoice);
            return Task.CompletedTask;
        }
    }
}
