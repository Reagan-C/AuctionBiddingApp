namespace PaymentService.Dto.PaystackResponse
{
    public class TransactionData
    {
        public string Reference { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PaidAt { get; set; }
        public TransactionMetadata Metadata { get; set; }
    }
}