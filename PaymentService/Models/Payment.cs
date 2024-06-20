
namespace PaymentService.Models
{
    public class Payment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
