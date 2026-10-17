using CwEstate.Inventory.Data;
using CwEstate.Inventory.Messages;
using CwEstate.Inventory.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Inventory.Consumers;

/// Commits the reservations an order holds once the order is placed.
public class OrderPlacedConsumer(InventoryDbContext db) : IConsumer<OrderPlaced>
{
    private readonly InventoryDbContext _db = db;

    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var ct = context.CancellationToken;
        var held = await _db.Reservations
            .Where(r => r.OrderId == context.Message.OrderId && r.State == ReservationState.Held)
            .ToListAsync(ct);
        foreach (var reservation in held)
        {
            reservation.State = ReservationState.Committed;
            var stock = await _db.StockItems.FirstOrDefaultAsync(s => s.Sku == reservation.Sku, ct);
            if (stock is not null)
            {
                stock.OnHand -= reservation.Quantity;
                stock.Reserved -= reservation.Quantity;
            }
        }
        await _db.SaveChangesAsync(ct);
    }
}
