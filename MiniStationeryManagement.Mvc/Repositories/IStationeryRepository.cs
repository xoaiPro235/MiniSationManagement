namespace MiniStationeryManagement.Mvc.Repositories;

using MiniStationeryManagement.Mvc.Models;

public interface IStationeryRepository
{
    Task<List<StationeryItem>> GetAllAsync();
    Task<StationeryItem?> GetByIdAsync(int id);

    Task<List<StationeryItem>> GetFilteredAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    );
    Task UpdateStockAsync(int itemId, int quantity);
}
