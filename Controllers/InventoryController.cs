using CwEstate.Inventory.Clients;
using CwEstate.Inventory.Data;
using CwEstate.Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Inventory.Controllers;

public record ReservationRequest(Guid OrderId, string Sku, int Quantity);
public record ReservationResult(Guid ReservationId, bool Reserved);
public record StockLevel(string Sku, int OnHand, int Reserved);

[ApiController]
[Route("api/inventory")]
public class InventoryController(InventoryDbContext db, CatalogClient catalog) : ControllerBase
{
    private readonly InventoryDbContext _db = db;
    private readonly CatalogClient _catalog = catalog;

    [HttpGet("stock")]
    public async Task<List<StockLevel>> AllStock(CancellationToken ct) =>
        await _db.StockItems.AsNoTracking().Select(s => new StockLevel(s.Sku, s.OnHand, s.Reserved)).ToListAsync(ct);

    [HttpGet("stock/{sku}")]
    public async Task<ActionResult<StockLevel>> Stock(string sku, CancellationToken ct)
    {
        var item = await _db.StockItems.FirstOrDefaultAsync(s => s.Sku == sku, ct);
        return item is null ? NotFound() : new StockLevel(item.Sku, item.OnHand, item.Reserved);
    }

    [HttpPost("reservations")]
    public async Task<ActionResult<ReservationResult>> Reserve(ReservationRequest request, CancellationToken ct)
    {
        var item = await _db.StockItems.FirstOrDefaultAsync(s => s.Sku == request.Sku, ct);
        if (item is null || item.Available < request.Quantity)
            return new ReservationResult(Guid.Empty, false);
        var reservation = new Reservation { Id = Guid.NewGuid(), OrderId = request.OrderId, Sku = request.Sku, Quantity = request.Quantity, CreatedAt = DateTime.UtcNow };
        item.Reserved += request.Quantity;
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync(ct);
        return new ReservationResult(reservation.Id, true);
    }

    /// Creates a zero-stock row for every catalog SKU that has none yet.
    [HttpPost("seed")]
    public async Task<ActionResult<int>> Seed(CancellationToken ct)
    {
        var products = await _catalog.ListProductsAsync(ct) ?? new();
        var known = await _db.StockItems.Select(s => s.Sku).ToListAsync(ct);
        var added = 0;
        foreach (var p in products.Where(p => !known.Contains(p.Sku)))
        {
            _db.StockItems.Add(new StockItem { Id = Guid.NewGuid(), Sku = p.Sku });
            added++;
        }
        await _db.SaveChangesAsync(ct);
        return added;
    }
}
