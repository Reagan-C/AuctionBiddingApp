using BiddingService.Dtos;
using BiddingService.Models;
using BiddingService.RabbitMq;
using BiddingService.Repository;
using BiddingService.Utilities;

namespace BiddingService.Services
{
    public class BidService : IBidService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBidRepository _bidRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BidService> _logger;

        public BidService(IPublishEndpoint publishEndpoint, IBidRepository bidRepository,
            IConfiguration configuration, ILogger<BidService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _bidRepository = bidRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BidResult> PlaceBid(string userId, PlaceBidRequest request)
        {
            var auction = await _bidRepository.GetAuctionAsync(request.AuctionId);
            if (auction == null)
                return new BidResult { Success = false, Message = "Auction not found" };

            if (auction.Status != AuctionStatus.InProgress)
                return new BidResult { Success = false, Message = "Auction has ended" };
            // Validate the bid
            var currentHighestBid = _bidRepository.GetHighestBidAmount(request.AuctionId);

            if (request.BidAmount <= currentHighestBid)
                return new BidResult { Success = false, Message = "Bid amount must be higher than the current highest bid." };

            // Create a new bid
            var bid = new Bid
            {
                AuctionId = request.AuctionId,
                UserId = userId,
                ItemName = auction.ItemName,
                Amount = request.BidAmount,
                Timestamp = DateTime.UtcNow
            };

            auction.Bids.Add(bid);

            await _bidRepository.SaveBidAsync(bid);
            await _bidRepository.UpdateAuctionAsync(auction);
            _logger.LogInformation("Bid placed, publishing to message queue...");

            // Publish the bid placed event to the Notification Service
            var queueName = _configuration["RabbitMq:BidPlaced"];
            await _publishEndpoint.Publish(bid, queueName: queueName);
            _logger.LogInformation("Published");
            return new BidResult { Success = true, Message = "Bid placed successfuly" };
        }

        public async Task<EndAuctionResult> EndAuction(int auctionId)
        {
            // Retrieve the auction from the database
            var auction = await _bidRepository.GetAuctionAsync(auctionId);

            if (auction == null)
                return new EndAuctionResult { Success = false, Message = $"Auction with ID {auctionId} not found."};
            
            if (auction.Status != AuctionStatus.InProgress)
                return new EndAuctionResult { Success = false, Message = "Auction is not in progress."};

            // Determine the winning bidder
            var winningBid = await _bidRepository.GetWinningBid(auctionId);

            // Update the auction status and winning bid information
            auction.Status = AuctionStatus.Closed;
            auction.EndTime = DateTime.UtcNow;
            await _bidRepository.UpdateAuctionAsync(auction);
            _logger.LogInformation("Auction ended, publishing to broker...");
            // Publish the auction ended event to the Notification Service
            var queueName = _configuration["RabbitMq:AuctionEnded"];
            await _publishEndpoint.Publish(winningBid, queueName: queueName);
            _logger.LogInformation("Published!!!");
            return new EndAuctionResult { Success = true, Message = "Auction ended successfully" };
        }
    }
}
