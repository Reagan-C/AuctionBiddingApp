
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentService.Dto;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(IPaymentService paymentService, ILogger<PaymentWebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var json = await reader.ReadToEndAsync();
                var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();

                if (!VerifySignature(json, signature))
                {
                    _logger.LogWarning("Invalid Paystack signature.");
                    return Unauthorized();
                }

                var webhookEvent = JObject.Parse(json);
                var eventType = webhookEvent["event"].ToString();
                _logger.LogInformation($"Received Paystack event: {eventType}");

                switch (eventType)
                {
                    case "charge.success":
                        var data = webhookEvent["data"];
                        var metadataJson = data["metadata"].ToString();
                        var metadata = JsonConvert.DeserializeObject<dynamic>(metadataJson);

                        var cancelAuctionUrl = metadata.cancel_auction;
                        var invoiceId = metadata.invoice_id;
                        var paymentData = webhookEvent["data"].ToObject<PaystackPaymentData>();
                        await _paymentService.ProcessPaymentAsync(paymentData);
                        _logger.LogInformation($"Processed payment with reference: {paymentData.Reference}");
                        break;

                    // Handle other event types as needed

                    default:
                        _logger.LogInformation($"Unhandled Paystack event: {eventType}");
                        break;
                }

                return Ok();
            }
        }
         
    }


}
