namespace NotificationService.Dtos
{
    public class InvoiceRequest
    {
        public int AuctionId { get; set; }
        public string BidderId { get; set; } = string.Empty;
        public string BidItemName { get; set; } = string.Empty;
        public decimal WinningBidAmount { get; set; }

    }
}
