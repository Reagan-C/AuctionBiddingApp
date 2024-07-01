using PaymentService.Services;

namespace PaymentService.Infrastructure.Messaging
{
    public class InvoiceProcessorService : BackgroundService
    {
        private readonly IInvoiceProcessor _invoiceProcessor;
        private readonly ILogger<InvoiceProcessorService> _logger;

        public InvoiceProcessorService(IInvoiceProcessor invoiceProcessor,
            ILogger<InvoiceProcessorService> logger)
        {
            _invoiceProcessor = invoiceProcessor;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Invoice Processor Service is starting.");

            _invoiceProcessor.StartConsuming();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoice Processor Service is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
