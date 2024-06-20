
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.Utilities;

namespace PaymentService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _context.Invoices.FindAsync(invoiceId) 
                ?? new Invoice();
        }

        public async Task<bool> IsPaymentProcessed(string transactionRef)
        {
                return await _context.Payments.AnyAsync(p => p.TransactionRef == transactionRef);
        }

        public async Task SaveInvoiceAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return false;
            }

            invoice.Status = status;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice> FindInvoiceByAuctionIdAsync(int auctionId)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.AuctionId == auctionId);
        }
    }
}
