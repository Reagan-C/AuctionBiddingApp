using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentService.Infrastructure.config;
using PaymentService.Models;
using PaymentService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaymentService.Services
{
    public class InvoiceProcessor : IInvoiceProcessor
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceProcessor> _logger;
        private readonly string _invoiceQueue;

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

        public async Task ProcessInvoiceAsync(string message)
        {
            try
            {
                var invoice = JsonConvert.DeserializeObject<Invoice>(message);
                using var scope = _serviceProvider.CreateScope();
                var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                await paymentRepository.SaveInvoiceAsync(invoice);
                _logger.LogInformation($"Invoice saved. Details: {invoice}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the invoice");
            }
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Task.Run(() => ProcessInvoiceAsync(message));
            };
            _channel.BasicConsume(queue: _invoiceQueue, autoAck: true, consumer: consumer);
        }
    }
}