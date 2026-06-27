namespace MiniStationeryManagement.Mvc.Repositories;

using MiniStationeryManagement.Mvc.Models;

public interface IStationeryRepository
{
    Task<List<StationeryItem>> GetAllAsync();
    Task<StationeryItem?> GetByIdAsync(int id);
    Task<StationeryItem?> GetByIdIgnoreFilterAsync(int id);
    Task<List<StationeryItem>> GetTrashedAsync();

    Task<List<StationeryItem>> GetFilteredAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    );
    Task UpdateStockAsync(int itemId, int quantity);
    Task<List<StationeryCategory>> GetAllCategoriesAsync();
    Task<List<StationeryCategory>> GetCategoriesWithItemsAsync();
    Task AddAsync(StationeryItem item);
    Task UpdateAsync(StationeryItem item, uint originalRowVersion);
    Task SaveChangesAsync();
    Task<List<StationeryItem>> SearchAsync(string? keyword, int? categoryId, string? stockStatus);
    Task<bool> SkuExistsIgnoreFiltersAsync(string sku, int? excludeId = null);
}
