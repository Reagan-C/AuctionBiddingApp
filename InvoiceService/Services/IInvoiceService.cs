using InvoiceService.Models;

namespace InvoiceService.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoiceDetails);
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
    }
}
