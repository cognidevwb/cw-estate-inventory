using CwEstate.Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Inventory.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>().HasIndex(s => s.Sku).IsUnique();
        modelBuilder.Entity<Reservation>().HasIndex(r => new { r.OrderId, r.Sku });
    }
}
