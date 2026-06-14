using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly IStationeryService _stationeryService;

    public StationeryController(IStationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    // GET: /Stationery hoặc /Stationery/Index
    public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var items = await _stationeryService.GetFilteredListAsync(categoryId, minPrice, maxPrice);
        return View(items);
    }

    // GET: /Stationery/Detail/{id}
    public async Task<IActionResult> Detail(int id)
    {
        var item = await _stationeryService.GetDetailByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    // GET: /Stationery/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _stationeryService.GetAllCategoriesAsync();
        return View(new StationeryCreateViewModel());
    }

    // POST: /Stationery/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StationeryCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _stationeryService.CreateItemAsync(model);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _stationeryService.GetAllCategoriesAsync();
        return View(model);
    }

    // GET: /Stationery/Search
    public IActionResult Search()
    {
        return View();
    }

    // GET/POST: /Stationery/PerformSearch (Dùng cho AJAX)
    public async Task<IActionResult> PerformSearch(string? keyword, int? categoryId)
    {
        var results = await _stationeryService.SearchItemsAsync(keyword, categoryId);
        return PartialView("_StationerySearchResultTable", results);
    }
}
