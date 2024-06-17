using InvoiceService.Models;
using InvoiceService.Repositories;
using InvoiceService.Utilities;

namespace InvoiceService.Services
{
    public class InvoiceServiceImpl : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceServiceImpl> _logger;

        public InvoiceServiceImpl(IInvoiceRepository invoiceRepository, ILogger<InvoiceServiceImpl> logger)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoiceDetails)
        {
            try
            {
                var invoice = new Invoice
                {
                    AuctionId = invoiceDetails.AuctionId,
                    BidderId = invoiceDetails.BidderId,
                    WinningBidAmount = invoiceDetails.WinningBidAmount,
                    BidItemName = invoiceDetails.BidItemName,
                    CreatedAt = DateTime.UtcNow,
                    Status = InvoiceStatus.Pending
                };

                await _invoiceRepository.CreateInvoiceAsync(invoice);
                _logger.LogInformation("Invoice created successfully. Invoice ID: {InvoiceId}", invoice.Id);

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the invoice.");
                throw;
            }
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
        }

        public async Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var updatedInvoice = await _invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, status);
            if (updatedInvoice == false)
            {
                _logger.LogError($"update operation unsuccessful for invoice {invoiceId}");
            }
  
            _logger.LogInformation($"Invoice with id {invoiceId} updated");
        }

    }
}
