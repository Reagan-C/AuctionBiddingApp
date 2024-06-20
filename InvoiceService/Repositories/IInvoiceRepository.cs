using InvoiceService.Models;

namespace InvoiceService.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
        Task<bool> CreateInvoiceAsync(Invoice invoice);
    }
}
