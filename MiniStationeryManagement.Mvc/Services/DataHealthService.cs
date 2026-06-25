using MiniStationeryManagement.Mvc.Repositories;
using MiniStationeryManagement.Mvc.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Services;

public class DataHealthService : IDataHealthService
{
    private readonly IDataHealthRepository _repository;

    public DataHealthService(IDataHealthRepository repository)
    {
        _repository = repository;
    }

    public async Task<DataHealthViewModel> CheckHealthAsync()
    {
        var model = new DataHealthViewModel();

        // 1. Kiểm tra migrations
        try
        {
            var pendingMigrations = await _repository.GetPendingMigrationsAsync();
            model.MigrationStatus = !pendingMigrations.Any()
                ? "HEALTHY (Database đã đồng bộ tất cả các bản thiết kế Migrations)"
                : $"WARNING (Phát hiện thấy {pendingMigrations.Count} bản thiết kế chưa update)";
        }
        catch (Exception ex)
        {
            model.MigrationStatus = $"UNHEALTHY (Lỗi kết nối cơ sở dữ liệu: {ex.Message})";
        }

        // 2. Kiểm tra seed data
        try
        {
            var hasCategories = await _repository.HasCategoriesAsync();
            var hasItems = await _repository.HasItemsAsync();
            model.SeedDataStatus = (hasCategories && hasItems)
                ? "HEALTHY (Dữ liệu Seed danh mục và sản phẩm mẫu hoạt động sẵn sàng)"
                : "WARNING (Database trống, hệ thống chưa nạp dữ liệu mẫu)";
        }
        catch
        {
            model.SeedDataStatus = "ERROR";
        }

        // 3. Kiểm tra AsNoTracking
        try
        {
            await _repository.TestNoTrackingAsync();
            model.NoTrackingStatus = "ENABLED (Đã áp dụng thành công AsNoTracking() giúp giải phóng bộ nhớ đệm)";
        }
        catch
        {
            model.NoTrackingStatus = "DISABLED (Lỗi kiểm tra AsNoTracking)";
        }

        // 4. Kiểm tra transaction
        try
        {
            var success = await _repository.TestTransactionAsync();
            model.TransactionStatus = success
                ? "HEALTHY (Cơ chế Commit/Rollback bảo vệ an toàn dữ liệu hoạt động tốt)"
                : "UNHEALTHY (Lỗi khi chạy transaction)";
        }
        catch (Exception ex)
        {
            model.TransactionStatus = $"FAILED (Lỗi thực thi chuỗi Transaction: {ex.Message})";
        }

        return model;
    }
}
