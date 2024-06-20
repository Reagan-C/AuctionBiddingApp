using PaymentService.Utilities;

namespace PaymentService.Models
{
    public class Invoice
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int InvoiceId { get; set; }
        public int AuctionId { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public InvoiceStatus Status { get; set; }


        public override string ToString()
        {
            return ($"Id: {Id}, AuctionId: {AuctionId}, BidderId: {BuyerId}, BidItemName: {ItemName}, WinningBidAmount: {Amount}, CreatedAt: {CreatedAt}");
        }
    }
}
