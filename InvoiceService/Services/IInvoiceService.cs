using InvoiceService.Models;
using InvoiceService.Utilities;

namespace InvoiceService.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoiceDetails);
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
        Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status);
    }
}
