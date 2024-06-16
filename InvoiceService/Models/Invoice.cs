using InvoiceService.Utilities;

namespace InvoiceService.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public string WinningBidderName { get; set; }
        public decimal WinningBidAmount { get; set; }
        public string ItemName { get; set; }
        public DateTime CreatedAt { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
