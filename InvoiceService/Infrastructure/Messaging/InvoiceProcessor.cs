using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using InvoiceService.Models;
using InvoiceService.Infrastructure.Config;
using InvoiceService.Services;

namespace InvoiceService.Infrastructure.Messaging
{
    public class InvoiceProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceProcessor> _logger;
        private readonly IConfiguration _configuration;
        private readonly RabbitMQConnection _rabbitMQConnection;

        public InvoiceProcessor(IServiceProvider serviceProvider, RabbitMQConnection rabbitMQConnection,
            ILogger<InvoiceProcessor> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQConnection = rabbitMQConnection;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var invoiceDetailsQueue = _configuration["RabbitMQ:InvoiceQueueName"];
            var paymentRequestQueue = _configuration["RabbitMQ:PaymentRequestQueueName"];

            var consumer = new EventingBasicConsumer(_rabbitMQConnection.Channel);
            consumer.Received += async (model, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var invoiceDetails = JsonConvert.DeserializeObject<Invoice>(message);

                    using var scope = _serviceProvider.CreateScope();
                    var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                    var invoice = await invoiceService.CreateInvoiceAsync(invoiceDetails);
                    
                    // Publish payment request to the Payment Service
                    var paymentRequest = new
                    {
                        InvoiceId = invoice.Id,
                        AuctionId = invoice.AuctionId,
                        ItemName = invoice.BidItemName,
                        BuyerId = invoice.BidderId,
                        Amount = invoice.WinningBidAmount
                    };
                    var paymentRequestAsJson = JsonConvert.SerializeObject(paymentRequest);
                    var paymentRequestBody = Encoding.UTF8.GetBytes(paymentRequestAsJson);
                    _rabbitMQConnection.Channel.BasicPublish("", paymentRequestQueue, body: paymentRequestBody);

                    _logger.LogInformation("Invoice processed successfully. Invoice ID: {InvoiceId}", invoice.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the invoice.");
                }
            };

            _rabbitMQConnection.Channel.BasicConsume(invoiceDetailsQueue, autoAck: true, consumer: consumer);
            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
