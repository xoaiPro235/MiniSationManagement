using MiniStationeryManagement.Mvc.Models;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Services;

public class StationeryService
{
    private readonly List<StationeryItem> _items =
    [
        new()
        {
            Id = 1,
            Sku = "B-TL-01",
            Name = "Bút bi Thiên Long",
            Category = "Bút",
            Supplier = "Thiên Long Group",
            Price = 5000,
            Quantity = 50,
            MinStock = 10,
            LastUpdatedAt = DateTime.Now.AddDays(-2),
        },
        new()
        {
            Id = 2,
            Sku = "V-CP-02",
            Name = "Vở Campus 200 trang",
            Category = "Vở",
            Supplier = "Campus VN",
            Price = 25000,
            Quantity = 2,
            MinStock = 5,
            LastUpdatedAt = DateTime.Now.AddDays(-1),
        },
        new()
        {
            Id = 3,
            Sku = "D-KG-03",
            Name = "Kẹp giấy Gim",
            Category = "Dụng cụ",
            Supplier = "Stacom",
            Price = 15000,
            Quantity = 0,
            MinStock = 5,
            LastUpdatedAt = DateTime.Now.AddDays(-5),
        },
        new()
        {
            Id = 4,
            Sku = "B-DQ-04",
            Name = "Bút dạ quang",
            Category = "Bút",
            Supplier = "Thiên Long Group",
            Price = 12000,
            Quantity = 15,
            MinStock = 4,
            LastUpdatedAt = DateTime.Now.AddHours(-12),
        },
    ];

    public List<StationeryItem> GetAll() => _items;

    public StationeryItem? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public StationeryStatsViewModel GetStats()
    {
        return new StationeryStatsViewModel
        {
            TotalItemTypes = _items.Count,
            TotalQuantity = _items.Sum(x => x.Quantity),
            TotalInventoryValue = _items.Sum(x => x.Quantity * x.Price),
            OutOfStockCount = _items.Count(x => x.Quantity <= 0),
            NeedReorderCount = _items.Count(x => x.Quantity > 0 && x.Quantity <= x.MinStock),
        };
    }

    public List<StationeryItem> Search(string? keyword, decimal? minPrice, string? supplier)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(item =>
                item.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || item.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || item.Sku.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (minPrice.HasValue)
        {
            query = query.Where((item => item.Price >= minPrice.Value));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(item =>
                item.Supplier.Contains(supplier, StringComparison.OrdinalIgnoreCase)
            );
        }
        return query.ToList();
    }

    public StationeryItem Create(StationeryCreateViewModel model)
    {
        var newId = _items.Count == 0 ? 1 : _items.Max(i => i.Id) + 1;

        var newItem = new StationeryItem
        {
            Id = newId,
            Sku = model.Sku.ToUpper(),
            Category = model.Category,
            Supplier = model.Supplier,
            Price = model.UnitPrice,
            Quantity = model.Quantity,
            MinStock = model.MinStock,
            LastUpdatedAt = DateTime.Now,
        };

        _items.Add(newItem);
        return newItem;
    }
}
