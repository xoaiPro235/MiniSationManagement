using System.ComponentModel.DataAnnotations;

namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryCreateViewModel
{
    [Required(ErrorMessage = "Tên văn phòng phẩm không được để trống")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Mã SKU bắt buộc phải nhập")]
    public string Sku { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập danh mục/nhóm sản phẩm")]
    public string Category { get; set; } = "";

    [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
    public string Supplier { get; set; } = "";

    [Range(
        100,
        50000000,
        ErrorMessage = "Giá bán phải nằm trong khoảng 1.000VND đến 50.000.000VND"
    )]
    public decimal UnitPrice { get; set; }

    [Range(0, 50000, ErrorMessage = "Số lượng tồn kho phải từ 0 đến 50.000")]
    public int Quantity { get; set; }

    [Range(0, 5000, ErrorMessage = "Mức tồn tối thiểu hợp lệ từ 0 đến 5.000")]
    public int MinStock { get; set; }
}
