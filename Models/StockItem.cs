namespace CwEstate.Inventory.Models;

public class StockItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available => OnHand - Reserved;
}
