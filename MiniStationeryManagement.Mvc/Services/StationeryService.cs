using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniStationeryManagement.Mvc.Models;
using MiniStationeryManagement.Mvc.Options;
using MiniStationeryManagement.Mvc.Repositories;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Services;

public class StationeryService : IStationeryService
{
    private readonly IStationeryRepository _stationeryRepository;
    private readonly AppSettings _appSettings;
    private readonly ILogger<StationeryService> _logger;

    public StationeryService(
        IStationeryRepository stationeryRepository,
        IOptions<AppSettings> appSettings,
        ILogger<StationeryService> logger
    )
    {
        _stationeryRepository = stationeryRepository;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<List<StationeryListItemViewModel>> GetFilteredListAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    )
    {
        var items = await _stationeryRepository.GetFilteredAsync(categoryId, minPrice, maxPrice);

        return items
            .Select(item => new StationeryListItemViewModel
            {
                Id = item.Id,
                Sku = item.Sku,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                Supplier = item.Supplier,
                CategoryName = item.Category?.Name ?? "Chưa phân loại",
                Barcode = item.Barcode,
                IsLowStock = item.Quantity <= _appSettings.LowStockThreshold,
            })
            .ToList();
    }

    public async Task<StationeryDetailViewModel?> GetDetailByIdAsync(int id)
    {
        var item = await _stationeryRepository.GetByIdAsync(id);
        if (item == null)
        {
            _logger.LogWarning("Stationery item not found when getting detail. ItemId={ItemId}", id);
            return null;
        }

        return new StationeryDetailViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity,
            Supplier = item.Supplier,
            Barcode = item.Barcode,
            CategoryName = item.Category?.Name ?? "Chưa phân loại",
        };
    }

    public async Task<List<StationeryCategoryViewModel>> GetAllCategoriesAsync()
    {
        var categories = await _stationeryRepository.GetAllCategoriesAsync();
        return categories
            .Select(c => new StationeryCategoryViewModel { Id = c.Id, Name = c.Name })
            .ToList();
    }

    public async Task<List<CategoryRelationshipViewModel>> GetCategoryRelationshipsAsync()
    {
        var categories = await _stationeryRepository.GetCategoriesWithItemsAsync();
        return categories
            .Select(c => new CategoryRelationshipViewModel
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                ProductsList = c.StationeryItems.Any()
                    ? string.Join(", ", c.StationeryItems.Select(item => item.Name))
                    : "Không có sản phẩm",
                RelationshipType = "1 - Many",
                DbSetName = "StationeryCategories"
            })
            .ToList();
    }

    public async Task CreateItemAsync(StationeryCreateViewModel model)
    {
        var newItem = new StationeryItem
        {
            Sku = model.Sku,
            Barcode = model.Barcode,
            Name = model.Name,
            Price = model.Price,
            Quantity = model.Quantity,
            MinStock = model.MinStock,
            Supplier = model.Supplier,
            CategoryId = model.CategoryId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _stationeryRepository.AddAsync(newItem);
        await _stationeryRepository.SaveChangesAsync();

        _logger.LogInformation("Stationery item created successfully. ItemId={ItemId}, Sku={Sku}", newItem.Id, newItem.Sku);
    }

    public async Task<List<StationeryListItemViewModel>> SearchItemsAsync(
        string? keyword,
        int? categoryId,
        string? stockStatus
    )
    {
        var items = await _stationeryRepository.SearchAsync(keyword, categoryId, stockStatus);

        return items
            .Select(item => new StationeryListItemViewModel
            {
                Id = item.Id,
                Sku = item.Sku,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                Supplier = item.Supplier,
                Barcode = item.Barcode,
                CategoryName = item.Category?.Name ?? "Chưa phân loại",
                IsLowStock = item.Quantity <= item.MinStock,
            })
            .ToList();
    }

    public async Task<StationeryEditViewModel?> GetEditDetailAsync(int id)
    {
        var item = await _stationeryRepository.GetByIdAsync(id);
        if (item == null)
            return null;

        return new StationeryEditViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Barcode = item.Barcode,
            Name = item.Name,
            Supplier = item.Supplier,
            Quantity = item.Quantity,
            MinStock = item.MinStock,
            Price = item.Price,
            CategoryId = item.CategoryId,
            RowVersion = item.RowVersion.ToString()
        };
    }

    public async Task UpdateItemAsync(StationeryEditViewModel model)
    {
        var item = await _stationeryRepository.GetByIdAsync(model.Id);
        if (item == null)
        {
            _logger.LogWarning("Stationery item not found for update. ItemId={ItemId}", model.Id);
            throw new KeyNotFoundException($"Item with ID {model.Id} not found");
        }

        item.Name = model.Name;
        item.Sku = model.Sku;
        item.Barcode = model.Barcode;
        item.Price = model.Price;
        item.Quantity = model.Quantity;
        item.MinStock = model.MinStock;
        item.Supplier = model.Supplier;
        item.CategoryId = model.CategoryId;
        item.UpdatedAt = DateTime.UtcNow;
        item.LastUpdatedAt = DateTime.UtcNow;

        uint originalRowVersion = uint.Parse(model.RowVersion);
        await _stationeryRepository.UpdateAsync(item, originalRowVersion);

        _logger.LogInformation("Stationery item updated. ItemId={ItemId}, Sku={Sku}", item.Id, item.Sku);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var item = await _stationeryRepository.GetByIdAsync(id);
        if (item == null)
        {
            _logger.LogWarning("Stationery item not found for soft delete. ItemId={ItemId}", id);
            return false;
        }

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        await _stationeryRepository.SaveChangesAsync();
        _logger.LogWarning("Stationery item soft deleted. ItemId={ItemId}, Sku={Sku}", item.Id, item.Sku);
        return true;
    }

    public async Task<List<StationeryTrashItemViewModel>> GetTrashedItemsAsync()
    {
        var trashed = await _stationeryRepository.GetTrashedAsync();
        return trashed
            .Select(t => new StationeryTrashItemViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Sku = t.Sku,
                DeletedAt = t.DeletedAt
            })
            .ToList();
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var item = await _stationeryRepository.GetByIdIgnoreFilterAsync(id);
        if (item == null || !item.IsDeleted)
        {
            _logger.LogWarning("Stationery item not found or not deleted for restore. ItemId={ItemId}", id);
            return false;
        }

        item.IsDeleted = false;
        item.DeletedAt = null;
        item.UpdatedAt = DateTime.UtcNow;

        await _stationeryRepository.SaveChangesAsync();
        _logger.LogInformation("Stationery item restored. ItemId={ItemId}, Sku={Sku}", item.Id, item.Sku);
        return true;
    }

    public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
    {
        return await _stationeryRepository.SkuExistsIgnoreFiltersAsync(sku, excludeId);
    }

    public async Task<StationeryAdjustStockViewModel?> GetAdjustStockViewModelAsync(int id)
    {
        var item = await _stationeryRepository.GetByIdAsync(id);
        if (item == null)
            return null;

        return new StationeryAdjustStockViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Sku = item.Sku,
            CurrentQuantity = item.Quantity,
            AdjustmentQuantity = 0,
            RowVersion = item.RowVersion.ToString()
        };
    }

    public async Task AdjustStockAsync(StationeryAdjustStockViewModel model)
    {
        var item = await _stationeryRepository.GetByIdAsync(model.Id);
        if (item == null)
        {
            _logger.LogWarning("Stationery item not found for stock adjustment. ItemId={ItemId}", model.Id);
            throw new KeyNotFoundException($"Item with ID {model.Id} not found");
        }

        int targetQuantity = item.Quantity + model.AdjustmentQuantity;
        if (targetQuantity < 0)
        {
            throw new ArgumentException("Số lượng tồn kho sau khi điều chỉnh không thể nhỏ hơn 0.");
        }

        item.Quantity = targetQuantity;
        item.UpdatedAt = DateTime.UtcNow;
        item.LastUpdatedAt = DateTime.UtcNow;

        uint originalRowVersion = uint.Parse(model.RowVersion);
        await _stationeryRepository.UpdateAsync(item, originalRowVersion);

        _logger.LogInformation("Stationery stock adjusted. ItemId={ItemId}, Sku={Sku}, Adjustment={Adjustment}, NewQuantity={NewQuantity}",
            item.Id, item.Sku, model.AdjustmentQuantity, item.Quantity);
    }
}
