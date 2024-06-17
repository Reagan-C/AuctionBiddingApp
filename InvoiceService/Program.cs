using InvoiceService.Data;
using InvoiceService.Infrastructure.Config;
using InvoiceService.Infrastructure.Messaging;
using InvoiceService.Repositories;
using InvoiceService.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog(config =>
    config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddDbContext<InvoiceDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceServiceImpl>();
builder.Services.AddHostedService<InvoiceProcessor>();

var host = builder.Build();

host.Run();
