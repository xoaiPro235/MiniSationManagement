using System.ComponentModel.DataAnnotations;

namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryAdjustStockViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }

    [Required(ErrorMessage = "Số lượng điều chỉnh là bắt buộc.")]
    [Range(-10000, 10000, ErrorMessage = "Số lượng điều chỉnh từ -10.000 đến 10.000.")]
    public int AdjustmentQuantity { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;
}
