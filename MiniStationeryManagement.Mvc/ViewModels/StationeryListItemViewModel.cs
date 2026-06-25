namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryListItemViewModel
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public bool IsLowStock { get; set; }
}
