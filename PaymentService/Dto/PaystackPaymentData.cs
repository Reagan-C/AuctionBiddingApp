namespace PaymentService.Dto
{
    public class PaystackPaymentData
    {
        public string Reference { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public int InvoiceId { get; set; }
    }
}
