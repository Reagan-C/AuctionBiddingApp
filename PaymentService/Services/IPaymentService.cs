
using PaymentService.Dto;
using PaymentService.Dto.PaystackResponse;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(string userId, PaymentRequest paymentRequest);
        Task<bool> ProcessCallbackAsync(string reference);
        Task<bool> ProcessWebhookPaymentAsync(PaystackEvent paystackEvent);
        bool VerifySignature(string jsonPayload, string actualSignature);
    }
}