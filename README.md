# cw-estate-inventory

Stock levels and reservations for the estate.

- ASP.NET Core 9 Web API, EF Core (SQLite), MassTransit over RabbitMQ.
- Owns `StockItems` and `Reservations`.
- Calls `cw-estate-catalog` to seed SKUs.
- Consumes `OrderPlaced` to commit reservations.
- Called by `cw-estate-orders` and `cw-estate-admin`.

Run: `dotnet run` (listens on http://localhost:5003).
