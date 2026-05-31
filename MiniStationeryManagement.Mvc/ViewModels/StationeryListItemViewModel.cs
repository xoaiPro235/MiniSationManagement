namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryListItemViewModel
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Supplier { get; set; } = "";
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public decimal Price { get; set; }
    public string PriceText => $"{Price:N0} VND";
    public decimal InventoryValue => Price * Quantity;
    public string InventoryValueText => $"{InventoryValue:N0} VND";
    public string StockStatus
    {
        get
        {
            if (Quantity <= 0)
                return "Hết hàng";
            if (Quantity <= 5)
                return "Cần nhập thêm";
            if (Quantity >= 30)
                return "Tồn kho cao";
            return "Còn hàng";
        }
    }
    public string StockStatusClass
    {
        get
        {
            if (Quantity <= 0)
                return "badge badge-danger";
            if (Quantity <= 5)
                return "badge badge-warning";
            return "badge badge-success";
        }
    }
}
