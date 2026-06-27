using System.ComponentModel.DataAnnotations;

namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryCreateViewModel
{
    [Required(ErrorMessage = "Sku là bắt buộc.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Sku chỉ gồm chữ in hoa, số và dấu -.")]
    [StringLength(20, ErrorMessage = "Sku không được vượt quá 20 ký tự.")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã vạch (Barcode) là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Mã vạch không được vượt quá 50 ký tự.")]
    public string Barcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3 đến 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhà cung cấp là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Nhà cung cấp không được vượt quá 100 ký tự.")]
    public string Supplier { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Tồn kho phải từ 0 đến 100.000.")]
    public int Quantity { get; set; }

    [Range(0, 1000, ErrorMessage = "Mức tồn kho tối thiểu từ 0 đến 1.000.")]
    public int MinStock { get; set; }

    [Range(1000, 100000000, ErrorMessage = "Giá phải từ 1.000 đến 100.000.000.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc.")]
    public int CategoryId { get; set; }
}
