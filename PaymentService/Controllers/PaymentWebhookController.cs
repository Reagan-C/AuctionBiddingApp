﻿
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentService.Dto.PaystackResponse;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(IPaymentService paymentService, ILogger<PaymentWebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault();
                    var verifySignature = _paymentService.VerifySignature(requestBody, signature);

                    if (!verifySignature)
                    {
                        _logger.LogWarning("Signature verification failed");
                        return BadRequest("Signature verification failed");
                    }
                    var paystackEvent = JsonConvert.DeserializeObject<PaystackEvent>(requestBody);
                    _logger.LogInformation("Request body deserialized");
                    await _paymentService.ProcessWebhookPaymentAsync(paystackEvent);
                    return Ok("Payment completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception encountered while processing payment");
                return StatusCode(500, "Error encountered while processing payment");
            }
        }

    }


}
