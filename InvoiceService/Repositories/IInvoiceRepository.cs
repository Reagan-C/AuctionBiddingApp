using InvoiceService.Models;
using InvoiceService.Utilities;

namespace InvoiceService.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
        Task<bool> CreateInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status);
    }
}
