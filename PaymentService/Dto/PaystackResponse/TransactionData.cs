using Newtonsoft.Json;

namespace PaymentService.Dto.PaystackResponse
{
    public class TransactionData
    {
        public string Reference { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("paid_at")]
        public DateTime PaidAt { get; set; }
        public Metadata Metadata { get; set; }
        public CustomerData Customer { get; set; }
    }
}