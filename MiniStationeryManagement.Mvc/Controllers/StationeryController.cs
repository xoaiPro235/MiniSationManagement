using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Models;
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
        var items = _service.GetAll().Select(ToListItemViewModel).ToList();
        return View(items);
    }

    public IActionResult Detail(int id)
    {
        var item = _service.GetById(id);
        if (item == null)
        {
            return NotFound($"Không tìm thấy mặt hàng văn phòng phẩm có ID = {id}");
        }

        var viewModels = ToDetailViewModel(item);
        return View(viewModels);
    }

    [HttpGet]
    public IActionResult Search(string? keyword, decimal? minPrice, string? supplier)
    {
        var items = _service
            .Search(keyword, minPrice, supplier)
            .Select(ToListItemViewModel)
            .ToList();

        var viewModel = new StationerySearchViewModel
        {
            Keyword = keyword ?? "",
            MinPrice = minPrice,
            Supplier = supplier ?? "",
            StationeryItems = items,
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new StationeryCreateViewModel { Quantity = 1, MinStock = 1 };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(StationeryCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _service.Create(model);
        TempData["SuccessMessage"] = "Đã thêm mới văn phòng phẩm thành công vào hệ thống!";

        return RedirectToAction(nameof(Index));
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

    private static StationeryListItemViewModel ToListItemViewModel(StationeryItem item)
    {
        return new StationeryListItemViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Category = item.Category,
            Supplier = item.Supplier,
            Price = item.Price,
            Quantity = item.Quantity,
            MinStock = item.MinStock,
        };
    }

    private static StationeryDetailViewModel ToDetailViewModel(StationeryItem item)
    {
        return new StationeryDetailViewModel
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
    }
}
