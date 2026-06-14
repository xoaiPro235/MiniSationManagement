using System.ComponentModel.DataAnnotations;

namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationeryCreateViewModel
{
    [Required]
    public string Sku { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Supplier { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }
}
