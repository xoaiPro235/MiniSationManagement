namespace MiniStationeryManagement.Mvc.Models;

public class StationeryItem
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public string Barcode { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public StationeryCategory? Category { get; set; }
}
