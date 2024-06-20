using InvoiceService.Models;
using InvoiceService.Repositories;

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
                    CreatedAt = DateTime.UtcNow
                };

                await _invoiceRepository.CreateInvoiceAsync(invoice);
                _logger.LogInformation($"Invoice created successfully. Invoice ID: {invoice.Id}");

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
    }
}
