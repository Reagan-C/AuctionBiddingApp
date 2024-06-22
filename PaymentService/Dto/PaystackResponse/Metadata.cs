using Newtonsoft.Json;

namespace PaymentService.Dto.PaystackResponse
{
    public class Metadata
    {
        [JsonProperty("invoice_id")]
        public int InvoiceId { get; set; }
    }
}