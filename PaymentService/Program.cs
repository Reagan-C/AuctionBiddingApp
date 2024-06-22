using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Infrastructure.config;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Repositories;
using PaymentService.Services;
using PayStack.Net;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilog(config =>
    config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<PayStackApi>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var secretKey = configuration["Paystack:SecretKey"];
    return new PayStackApi(secretKey);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IInvoiceProcessor, InvoiceProcessor>();
builder.Services.AddHostedService<InvoiceProcessorHostedService>();

builder.Services.AddCors();

builder.Services.Configure<PaystackSettings>(builder.Configuration.GetSection("Paystack"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));


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

app.UseSerilogRequestLogging();
app.UseCors(builder => builder
    .WithOrigins("*")
    .AllowAnyOrigin()
    .AllowAnyMethod()
);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
