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

        public BidService(IPublishEndpoint publishEndpoint, IBidRepository bidRepository)
        {
            _publishEndpoint = publishEndpoint;
            _bidRepository = bidRepository;
        }

        public async Task PlaceBid(string userId, PlaceBidRequest request)
        {
            var auction = await _bidRepository.GetAuctionAsync(request.AuctionId);
            if (auction == null)
                throw new ArgumentException("Auction not found");
            // Validate the bid
            var currentHighestBid = _bidRepository.GetHighestBidAmount(request.AuctionId);

            if (request.BidAmount <= currentHighestBid)
            {
                throw new ArgumentException("Bid amount must be higher than the current highest bid.");
            }

            // Create a new bid
            var bid = new Bid
            {
                AuctionId = request.AuctionId,
                UserId = userId,
                Amount = request.BidAmount,
                Timestamp = DateTime.UtcNow
            };

            auction.Bids.Add(bid);

            await _bidRepository.SaveBidAsync(bid);
            await _bidRepository.SaveAuctionAsync(auction);

            // Publish the bid placed event to the Notification Service
            await _publishEndpoint.Publish(bid);
        }

        public async Task EndAuction(int auctionId)
        {
            // Retrieve the auction from the database
            var auction = await _bidRepository.GetAuctionAsync(auctionId);

            if (auction == null)
            {
                throw new ArgumentException($"Auction with ID {auctionId} not found.");
            }

            if (auction.Status != AuctionStatus.InProgress)
            {
                throw new ArgumentException("Auction is not in progress.");
            }

            // Determine the winning bidder
            var winningBid = await _bidRepository.GetWinningBid(auctionId);

            // Update the auction status and winning bid information
            auction.Status = AuctionStatus.Closed;
            await _bidRepository.SaveAuctionAsync(auction);
            // Publish the auction ended event to the Notification Service
            await _publishEndpoint.Publish(winningBid);
        }
    }
}
