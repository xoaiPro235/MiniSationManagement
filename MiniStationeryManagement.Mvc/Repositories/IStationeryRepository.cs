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
    Task<List<StationeryCategory>> GetAllCategoriesAsync();
    Task<List<StationeryCategory>> GetCategoriesWithItemsAsync();
    Task AddAsync(StationeryItem item);
    Task SaveChangesAsync();
    Task<List<StationeryItem>> SearchAsync(string? keyword, int? categoryId);
}
