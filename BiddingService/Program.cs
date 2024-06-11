using BiddingService.Data;
using BiddingService.Kafka;
using BiddingService.RabbitMq;
using BiddingService.Repository;
using BiddingService.Services.Impl;
using BiddingService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddDbContext<BiddingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IPublishEndpoint>(provider =>
{
    var factory = new ConnectionFactory { HostName = builder.Configuration["RabbitMQ:HostName"] };
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    var exchangeName = builder.Configuration["RabbitMQ:ExchangeName"];
    var exchangeType = builder.Configuration["RabbitMQ:ExchangeType"];
    channel.ExchangeDeclare(exchangeName, exchangeType);
    return new RabbitMQPublishEndpoint(channel, exchangeName);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
