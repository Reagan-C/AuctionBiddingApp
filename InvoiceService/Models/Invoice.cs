namespace InvoiceService.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public string BidderId { get; set; } = string.Empty;
        public string BidItemName { get; set; } = string.Empty;
        public decimal WinningBidAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
