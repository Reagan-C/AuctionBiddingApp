
using PaymentService.Dto;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(PaymentRequest paymentRequest);
        Task ProcessPaymentAsync(PaystackPaymentData paymentRequest);
    }
}