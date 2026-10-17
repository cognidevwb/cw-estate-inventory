using System.Net.Http.Json;

namespace CwEstate.Inventory.Clients;

public record CatalogProduct(Guid Id, string Sku, string Name, decimal Price);

/// Typed client for cw-estate-catalog, used to seed a stock row per SKU.
public class CatalogClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public Task<List<CatalogProduct>?> ListProductsAsync(CancellationToken ct) =>
        _http.GetFromJsonAsync<List<CatalogProduct>>("/api/products", ct);
}
