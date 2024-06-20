using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using PaymentService.Dto;
using PaymentService.Infrastructure.config;
using PaymentService.Models;
using PaymentService.Repositories;
using PayStack.Net;
using ContentType = MimeKit.ContentType;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaystackSettings _paystackSettings;
        private readonly EmailSettings _emailSettings;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly PayStackApi _payStackApi;

        public PaymentService(IOptions<PaystackSettings> paystackSettings, IOptions<EmailSettings> emailSettings,
            ILogger<PaymentService> logger, IPaymentRepository paymentRepository,
            PayStackApi payStackApi)
        {
            _paystackSettings = paystackSettings.Value;
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _payStackApi = payStackApi;
        }

        public async Task<string> InitiatePaymentAsync(PaymentRequest paymentRequest)
        {
            var invoice = await _paymentRepository.FindInvoiceByAuctionIdAsync(paymentRequest.AuctionId);
            if (invoice == null)
            {
                throw new Exception($"Invoice not found for auction ID: {paymentRequest.AuctionId}");
            }

            var request = new TransactionInitializeRequest
            {
                AmountInKobo = (int)Math.Round(invoice.Amount * 100, MidpointRounding.AwayFromZero),
                Email = paymentRequest.Email,
                Reference = GenerateUniqueReference(),
                Currency = "NGN",
                CallbackUrl = _paystackSettings.CallBackUrl,
                Metadata = JsonConvert.SerializeObject(new
                {
                    cancel_action = _paystackSettings.CancelActionUrl,
                    invoice_id = invoice.InvoiceId
                })
            };

            var response = _payStackApi.Transactions.Initialize(request);
            if (response.Status)
            {
                return response.Data.AuthorizationUrl;
            }
            else
            {
                throw new Exception($"Payment initialization failed: {response.Message}");
            }
        }

        public Task ProcessPaymentAsync(PaystackPaymentData paymentRequest)
        {
            throw new NotImplementedException();
        }

        public async Task VerifyPaymentCallbackAsync(string reference)
        {
            try
            {

                var response = _payStackApi.Transactions.Verify(reference);
                if (response.Status && response.Data.Status == "success")
                {
                    // var invoiceId = response.Data.Metadata.("invoice_id");

                    // Retrieve the invoice details based on the invoiceId
                    //var invoice = await GetInvoiceAsync(invoiceId);

                    // Update the invoice status to "Paid"
                    //await UpdateInvoiceStatusAsync(invoiceId, "Paid");

                    // Generate the PDF invoice
                    // var pdfBytes = await GenerateInvoicePdfAsync(invoice);

                    // Send email notification with the PDF invoice attached
                    // await SendInvoiceEmailAsync(invoice, pdfBytes);

                    //  _logger.LogInformation("Payment processed successfully for invoice {InvoiceId}", invoiceId);
                }
                else
                {
                    //  _logger.LogError("Failed to verify payment. Reference: {Reference}, Error: {Error}", reference, response.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment callback. Reference: {Reference}", reference);
            }
        }

        private async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Add invoice details to the PDF document
            var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var paragraphFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var title = new Paragraph("Invoice")
                .SetFont(titleFont)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(title);

            var invoiceNumber = new Paragraph($"Invoice Number: {invoice.Id}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(invoiceNumber);

            var auctionId = new Paragraph($"Auction ID: {invoice.AuctionId}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(auctionId);

            var winningBidder = new Paragraph($"Winning Bidder Id: {invoice.BuyerId}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(winningBidder);

            var winningBidAmount = new Paragraph($"Winning Bid Amount: {invoice.Amount}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(winningBidAmount);

            var itemName = new Paragraph($"Item Name: {invoice.ItemName}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(itemName);

            var createdAt = new Paragraph($"Created At: {invoice.CreatedAt}")
                .SetFont(paragraphFont)
                .SetFontSize(12)
                .SetMarginBottom(10);
            document.Add(createdAt);

            document.Close();

            return await Task.FromResult(ms.ToArray());
        }

        private async Task SendInvoiceEmailAsync(Invoice invoice, byte[] pdfBytes)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", invoice.BuyerId));
            message.Subject = "Invoice Details";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = "Please find the attached invoice.";
            bodyBuilder.Attachments.Add("invoice.pdf", pdfBytes, ContentType.Parse("application/pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, int.Parse(_emailSettings.SmtpPort), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Invoice email sent for invoice {InvoiceId}", invoice.Id);
        }

        private static string GenerateUniqueReference()
        {
            return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        }
    }
}
