namespace CwEstate.Inventory.Models;

public enum ReservationState { Held, Committed, Released }

public class Reservation
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
    public ReservationState State { get; set; } = ReservationState.Held;
    public DateTime CreatedAt { get; set; }
}
