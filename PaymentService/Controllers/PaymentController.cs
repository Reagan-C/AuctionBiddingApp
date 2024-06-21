using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PaymentService.Dto;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(PaymentRequest request)
        {
            try
            {
                var authorizationUrl = await _paymentService.InitiatePaymentAsync(request);
                return Ok(new { AuthorizationUrl = authorizationUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initiating payment");
                return StatusCode(500, "An error occurred while initiating payment");
            }
        }

        [HttpPost("callback")]
        public async Task<IActionResult> PaymentCallback([FromQuery] string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                _logger.LogWarning("Payment callback received without a reference.");
                return BadRequest("Invalid payment callback request.");
            }
            return Ok();
        }
    }
}
