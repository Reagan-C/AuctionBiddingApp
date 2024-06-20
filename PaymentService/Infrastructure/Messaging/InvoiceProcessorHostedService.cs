using PaymentService.Services;

namespace PaymentService.Infrastructure.Messaging
{
    public class InvoiceProcessorHostedService : IHostedService
    {
        private readonly IInvoiceProcessor _invoiceProcessor;
        private readonly ILogger<InvoiceProcessorHostedService> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public InvoiceProcessorHostedService(IInvoiceProcessor invoiceProcessor, 
            ILogger<InvoiceProcessorHostedService> logger)
        {
            _invoiceProcessor = invoiceProcessor;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting InvoiceProcessorHostedService");

            // Create a new CancellationTokenSource for the hosted service
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                // Start processing invoices
                await _invoiceProcessor.ProcessInvoicesAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting the invoice processor.");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping InvoiceProcessorHostedService");

            // Cancel the token to stop the invoice processing gracefully
            _cancellationTokenSource.Cancel();

            try
            {
                // Ensure any ongoing work is completed before fully stopping
                await _invoiceProcessor.ProcessInvoicesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while stopping the invoice processor.");
            }
        }
    }
}
