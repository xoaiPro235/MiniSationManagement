using Microsoft.Extensions.Options;
using MiniStationeryManagement.Mvc.Options;
using MiniStationeryManagement.Mvc.Repositories;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Services;

public class StationeryService
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
                IsLowStock = item.Quantity <= _appSettings.LowStockThreshold,
            })
            .ToList();
    }

    public async Task OrderStationeryAsync(string customerName, List<(int itemId, int qty)> items)
    {
        await _stationeryRepository.CreateOrderWithTransactionAsync(customerName, items);
    }
}
