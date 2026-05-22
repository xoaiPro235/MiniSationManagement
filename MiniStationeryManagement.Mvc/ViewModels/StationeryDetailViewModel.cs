namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryDetailViewModel
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Supplier { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public string PriceText => $"{Price:N0} VND";
    public decimal InventoryValue => Price * Quantity;
    public string InventoryValueText => $"{InventoryValue:N0} VND";
    public string LastUpdatedText => LastUpdatedAt.ToString("dd/MM/yyyy HH:mm");
    public string StockStatus
    {
        get
        {
            if (Quantity <= 0)
                return "Hết hàng";
            if (Quantity <= MinStock)
                return "Cần nhập thêm";
            return "Còn hàng";
        }
    }
    public string ReorderSuggestion
    {
        get
        {
            if (Quantity <= 0)
                return "Cần nhập hàng gấp vì văn phòng phẩm này đã hết sạch trong kho.";
            if (Quantity <= MinStock)
                return $"Nên làm đơn nhập thêm. Tồn kho hiện tại ({Quantity}) đang dưới mức tối thiểu ({MinStock}).";
            return "Số lượng tồn kho ổn định, đáp ứng tốt nhu cầu.";
        }
    }
}
