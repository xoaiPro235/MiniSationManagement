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

    public StationeryService(
        IStationeryRepository stationeryRepository,
        IOptions<AppSettings> appSettings
    )
    {
        _stationeryRepository = stationeryRepository;
        _appSettings = appSettings.Value;
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
            return null;

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

    public async Task CreateItemAsync(StationeryCreateViewModel model)
    {
        var newItem = new StationeryItem
        {
            Sku = model.Sku,
            Barcode = model.Barcode,
            Name = model.Name,
            Price = model.Price,
            Quantity = model.Quantity,
            Supplier = model.Supplier,
            CategoryId = model.CategoryId,
            LastUpdatedAt = DateTime.UtcNow,
        };

        await _stationeryRepository.AddAsync(newItem);
        await _stationeryRepository.SaveChangesAsync();
    }

    public async Task<List<StationeryListItemViewModel>> SearchItemsAsync(
        string? keyword,
        int? categoryId
    )
    {
        var items = await _stationeryRepository.SearchAsync(keyword, categoryId);

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
            })
            .ToList();
    }
}
