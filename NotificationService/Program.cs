
using NotificationService.EventConsumers;
using NotificationService.Hubs;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Interfaces;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddSingleton<IBidPlacedEventConsumer, BidPlacedEventConsumer>();
builder.Services.AddSingleton<IAuctionEndedEventConsumer, AuctionEndedEventConsumer>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<NotificationServiceImp>();
builder.Services.AddLogging();
var app = builder.Build();

app.MapHub<AuctionHub>("/auctionhub");

await app.RunAsync();
