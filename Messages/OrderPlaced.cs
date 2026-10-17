namespace CwEstate.Inventory.Messages;

public record OrderPlacedLine(string Sku, int Quantity);

/// The orders service's event, mirrored here (same name, same shape).
public record OrderPlaced(Guid OrderId, string CustomerEmail, List<OrderPlacedLine> Lines, DateTime PlacedAt);
