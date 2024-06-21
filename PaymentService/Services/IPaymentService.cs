
using PaymentService.Dto;
using PaymentService.Dto.PaystackResponse;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(PaymentRequest paymentRequest);
        Task<bool> ProcessWebhookPaymentAsync(PaystackEvent paystackEvent);
        bool VerifySignature(string jsonPayload, string actualSignature)
    }
}