using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Services;

public interface IStationeryService
{
    // Hàm lọc danh sách hiển thị ở trang Index
    Task<List<StationeryListItemViewModel>> GetFilteredListAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    );

    Task<StationeryDetailViewModel?> GetDetailByIdAsync(int id);

    Task<List<StationeryCategoryViewModel>> GetAllCategoriesAsync();

    Task CreateItemAsync(StationeryCreateViewModel model);

    Task<List<StationeryListItemViewModel>> SearchItemsAsync(string? keyword, int? categoryId);
}
