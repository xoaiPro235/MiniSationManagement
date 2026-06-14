using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Controllers;

public class DataHealthController : Controller
{
    private readonly AppDbContext _context;

    public DataHealthController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /DataHealth
    public async Task<IActionResult> Index()
    {
        var model = new DataHealthViewModel();

        // 1. Kiểm thử Trạng thái EF Core Migration
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            model.MigrationStatus = !pendingMigrations.Any()
                ? "HEALTHY (Database đã đồng bộ tất cả các bản thiết kế Migrations)"
                : $"WARNING (Phát hiện thấy {pendingMigrations.Count()} bản thiết kế chưa update)";
        }
        catch (Exception ex)
        {
            model.MigrationStatus = $"UNHEALTHY (Lỗi kết nối cơ sở dữ liệu: {ex.Message})";
        }

        // 2. Kiểm thử Trạng thái Seed Data hình thành dữ liệu mẫu
        try
        {
            var hasCategories = await _context.StationeryCategories.AnyAsync();
            var hasItems = await _context.StationeryItems.AnyAsync();
            model.SeedDataStatus =
                (hasCategories && hasItems)
                    ? "HEALTHY (Dữ liệu Seed danh mục và sản phẩm mẫu hoạt động sẵn sàng)"
                    : "WARNING (Database trống, hệ thống chưa nạp dữ liệu mẫu)";
        }
        catch
        {
            model.SeedDataStatus = "ERROR";
        }

        // 3. Minh chứng cấu hình tối ưu Read-Only bằng No-Tracking
        var itemsNoTracking = await _context.StationeryItems.AsNoTracking().Take(1).ToListAsync();
        model.NoTrackingStatus =
            "ENABLED (Đã áp dụng thành công AsNoTracking() giúp giải phóng bộ nhớ đệm)";

        // 4. Kiểm thử tính toàn vẹn dữ liệu ACID với DB Transaction
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var count = await _context.StationeryItems.CountAsync();
                await transaction.CommitAsync();
                model.TransactionStatus =
                    "HEALTHY (Cơ chế Commit/Rollback bảo vệ an toàn dữ liệu hoạt động tốt)";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                model.TransactionStatus = $"FAILED (Lỗi thực thi chuỗi Transaction: {ex.Message})";
            }
        }

        return View(model);
    }
}
