using CwEstate.Inventory.Clients;
using CwEstate.Inventory.Consumers;
using CwEstate.Inventory.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<InventoryDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Inventory")));
builder.Services.AddHttpClient<CatalogClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:Catalog"]!));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost");
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.MapControllers();
app.Run();
