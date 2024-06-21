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
using PaymentService.Dto.PaystackResponse;
using PaymentService.Infrastructure.config;
using PaymentService.Models;
using PaymentService.Repositories;
using PaymentService.Utilities;
using PayStack.Net;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<bool> ProcessWebhookPaymentAsync(PaystackEvent paystackEvent)
        {
            if (paystackEvent.Event == "charge.success")
            {
                var transactionData = paystackEvent.Data;

                _logger.LogInformation("Transaction Details: Reference={Reference}, Amount={Amount}, InvoiceId={InvoiceId}",
                    transactionData.Reference, transactionData.Amount, transactionData.Metadata.InvoiceId);

                var isProcessed = await _paymentRepository.IsPaymentProcessed(transactionData.Reference);

                if (isProcessed)
                {
                    _logger.LogWarning($"Invoice reference {transactionData.Reference} already processed");
                    return false;
                }

                if (transactionData.Status == "success" && transactionData.Metadata != null)
                {
                    var invoiceId = transactionData.Metadata.InvoiceId;
                    var updateStatus = await _paymentRepository.UpdateInvoiceStatusAsync(invoiceId, InvoiceStatus.Paid);

                    if (!updateStatus)
                    {
                        _logger.LogWarning($"Update operation failed for invoice with ID {invoiceId}");
                        return false;
                    }

                    var payment = new Payment
                    {
                        InvoiceId = invoiceId,
                        Amount = transactionData.Amount / 100.0m,
                        Currency = transactionData.Currency,
                        TransactionRef = transactionData.Reference,
                        CreatedAt = transactionData.CreatedAt,
                        PaidAt = transactionData.PaidAt,
                        Email = paystackEvent.Customer.Email
                    };

                    await _paymentRepository.SavePaymentAsync(payment);
                    await _paymentRepository.SaveChangesAsync();

                    var invoice = await _paymentRepository.GetInvoiceByIdAsync(invoiceId);
                    var pdfBytes = await GenerateInvoicePdfAsync(invoice);

                    await SendInvoiceEmailAsync(payment, pdfBytes);
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool VerifySignature(string jsonPayload, string actualSignature)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_paystackSettings.SecretKey));
            byte[] expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonPayload));

            byte[] actualHash = new byte[expectedHash.Length];
            if (!Convert.TryFromBase64String(actualSignature, actualHash, out _))
            {
                _logger.LogError("Signature ca not be verified");
                return false;
            }
            _logger.LogInformation("Signature verified");
            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
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
            _logger.LogInformation("Invoice PDF created");
            return await Task.FromResult(ms.ToArray());
        }

        private async Task SendInvoiceEmailAsync(Payment payment, byte[] pdfBytes)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", payment.Email));
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

            _logger.LogInformation("Invoice email sent for invoice {InvoiceId}", payment.InvoiceId);
        }

        private static string GenerateUniqueReference()
        {
            return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        }
    }
}
