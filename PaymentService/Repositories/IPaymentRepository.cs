using PaymentService.Models;
using PaymentService.Utilities;

namespace PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
        Task SaveInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status);
        Task SavePaymentAsync(Payment payment);
        Task<bool> IsPaymentProcessed(string transactionRef);
        Task<Invoice> FindInvoiceByAuctionIdAsync(int auctionId);
        Task SaveChangesAsync();
    }
}
