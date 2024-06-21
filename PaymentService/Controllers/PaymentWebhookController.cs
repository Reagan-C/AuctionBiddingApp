
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentService.Dto.PaystackResponse;
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
                var requestBody = await reader.ReadToEndAsync();
                var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault();
                var verifySignature = _paymentService.VerifySignature(requestBody, signature);

                if (verifySignature)
                {
                    var paystackEvent = JsonConvert.DeserializeObject<PaystackEvent>(requestBody);
                    await _paymentService.ProcessWebhookPaymentAsync(paystackEvent);
                }
            }
            return Ok();
        }

    }


}
