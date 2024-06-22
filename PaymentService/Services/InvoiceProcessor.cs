using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentService.Infrastructure.config;
using PaymentService.Models;
using PaymentService.Repositories;
using PaymentService.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaymentService.Services
{
    public class InvoiceProcessor : IInvoiceProcessor, IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceProcessor> _logger;
        private readonly string _invoiceQueue;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public InvoiceProcessor(IOptions<RabbitMQSettings> rabbitMQSettings,
            IServiceProvider serviceProvider, ILogger<InvoiceProcessor> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.Value.HostName,
                UserName = rabbitMQSettings.Value.UserName,
                Password = rabbitMQSettings.Value.Password,
                VirtualHost = rabbitMQSettings.Value.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _invoiceQueue = rabbitMQSettings.Value.InvoiceQueue;
            _channel.QueueDeclare(_invoiceQueue, durable: true, exclusive: false);

            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ProcessInvoicesAsync(CancellationToken cancellationToken)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, eventArgs) =>
            {
                if (linkedCts.Token.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    var body = eventArgs.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    dynamic receivedInvoice = JsonConvert.DeserializeObject(json);

                    var invoice = MapToProcessedInvoice(receivedInvoice);
                    using var scope = _serviceProvider.CreateScope();
                    var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    await paymentRepository.SaveInvoiceAsync(invoice);
                    _logger.LogInformation($"Invoice saved. Details: {invoice}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the invoice");
                }
            };

            _channel.BasicConsume(_invoiceQueue, autoAck: true, consumer: consumer);
        }

        private Invoice MapToProcessedInvoice(dynamic invoice)
        {
            return new Invoice
            {
                InvoiceId = invoice.InvoiceId,
                AuctionId = invoice.AuctionId,
                BuyerId = invoice.BuyerId,
                ItemName = invoice.ItemName,
                Amount = invoice.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Pending
            };
        }
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
