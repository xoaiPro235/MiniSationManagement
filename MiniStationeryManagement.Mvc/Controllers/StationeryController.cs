using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly StationeryService _service;

    public StationeryController(StationeryService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var items = _service
            .GetAll()
            .Select(i => new StationeryListItemViewModel
            {
                Id = i.Id,
                Sku = i.Sku,
                Name = i.Name,
                Category = i.Category,
                Supplier = i.Supplier,
                Price = i.Price,
                Quantity = i.Quantity,
            })
            .ToList();
        return View(items);
    }

    public IActionResult Detail(int id)
    {
        var item = _service.GetById(id);
        if (item == null)
        {
            return NotFound($"Không tìm thấy mặt hàng văn phòng phẩm có ID = {id}");
        }

        var viewModels = new StationeryDetailViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Category = item.Category,
            Supplier = item.Supplier,
            Price = item.Price,
            Quantity = item.Quantity,
            MinStock = item.MinStock,
            LastUpdatedAt = item.LastUpdatedAt,
        };

        return View(viewModels);
    }

    public IActionResult Stats()
    {
        var stats = _service.GetStats();
        return View(stats);
    }

    public IActionResult Welcome()
    {
        return Content("Chào mừng đến với Mini Stationery Management!");
    }

    public IActionResult StationeryJson()
    {
        var rawData = _service.GetAll();
        return Json(rawData);
    }

    public IActionResult GoToCatalog()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Force404()
    {
        return NotFound("404 NotFound");
    }
}
