﻿namespace NotificationService.Dtos
{
    public class PlacedBidResponse
    {
        public int AuctionId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
