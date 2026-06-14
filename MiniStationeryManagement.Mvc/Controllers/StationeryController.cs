using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;

namespace MiniStationeryManagement.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly StationeryService _stationeryService;

    public StationeryController(StationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var viewModel = await _stationeryService.GetFilteredListAsync(
            categoryId,
            minPrice,
            maxPrice
        );
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(string customerName, int itemId, int quantity)
    {
        try
        {
            var orderItems = new List<(int itemId, int qty)> { (itemId, quantity) };
            await _stationeryService.OrderStationeryAsync(customerName, orderItems);

            TempData["SuccessMessage"] =
                "Đặt hàng văn phòng phẩm thành công! Số lượng tồn kho đã được cập nhật tự động.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] =
                $"Giao dịch thất bại! Lỗi: {ex.Message} - Hệ thống đã kích hoạt cơ chế Rollback an toàn dữ liệu.";
        }
        return RedirectToAction(nameof(Index));
    }
}
