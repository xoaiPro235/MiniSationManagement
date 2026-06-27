using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Services;

public interface IStationeryService
{
    Task<List<StationeryListItemViewModel>> GetFilteredListAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    );

    Task<StationeryDetailViewModel?> GetDetailByIdAsync(int id);

    Task<List<StationeryCategoryViewModel>> GetAllCategoriesAsync();

    Task<List<CategoryRelationshipViewModel>> GetCategoryRelationshipsAsync();

    Task CreateItemAsync(StationeryCreateViewModel model);

    Task<List<StationeryListItemViewModel>> SearchItemsAsync(string? keyword, int? categoryId, string? stockStatus);

    Task<StationeryEditViewModel?> GetEditDetailAsync(int id);

    Task UpdateItemAsync(StationeryEditViewModel model);

    Task<bool> SoftDeleteAsync(int id);

    Task<List<StationeryTrashItemViewModel>> GetTrashedItemsAsync();

    Task<bool> RestoreAsync(int id);

    Task<bool> SkuExistsAsync(string sku, int? excludeId = null);

    Task<StationeryAdjustStockViewModel?> GetAdjustStockViewModelAsync(int id);

    Task AdjustStockAsync(StationeryAdjustStockViewModel model);
}
