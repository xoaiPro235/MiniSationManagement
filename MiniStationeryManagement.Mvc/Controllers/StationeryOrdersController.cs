using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;

namespace MiniStationeryManagement.Mvc.Controllers;

public class StationeryOrdersController : Controller
{
    private readonly IStationeryOrderService _orderService;

    // Chỉ inject dịch vụ Order hóa đơn đúng nguyên lý Single Responsibility
    public StationeryOrdersController(IStationeryOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST: /StationeryOrders/PlaceOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(string customerName, List<OrderItemInput> items)
    {
        if (string.IsNullOrEmpty(customerName) || items == null || !items.Any())
        {
            TempData["Error"] = "Thông tin đơn hàng hoặc tên khách hàng không hợp lệ.";
            return RedirectToAction("Index", "Stationery");
        }

        try
        {
            var orderItems = items.Select(i => (i.ItemId, i.Quantity)).ToList();

            // Thực hiện nghiệp vụ lưu hóa đơn và trừ tồn kho trong cùng 1 Transaction cô lập
            await _orderService.OrderStationeryAsync(customerName, orderItems);

            TempData["Success"] = "Đặt hàng thành công! Số lượng tồn kho đã được cập nhật tự động.";
        }
        catch (InvalidOperationException ex)
        {
            // Trả về lỗi Rollback khi không đủ hàng trong kho
            TempData["Error"] = $"Đặt hàng thất bại: {ex.Message}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Có lỗi hệ thống xảy ra: {ex.Message}";
        }

        return RedirectToAction("Index", "Stationery");
    }
}

public class OrderItemInput
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}
