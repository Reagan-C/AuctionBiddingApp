
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
            return await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId) 
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
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null)
            {
                return false;
            }

            invoice.Status = status;
            invoice.UpdatedAt = DateTime.UtcNow;
            _context.Invoices.Update(invoice);
            return true;
        }

        public async Task<Invoice> FindInvoiceByAuctionIdAsync(int auctionId)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.AuctionId == auctionId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
