using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniStationeryManagement.Mvc.Services;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly IStationeryService _stationeryService;
    private readonly ILogger<StationeryController> _logger;

    public StationeryController(IStationeryService stationeryService, ILogger<StationeryController> logger)
    {
        _stationeryService = stationeryService;
        _logger = logger;
    }

    // GET: /Stationery
    public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var items = await _stationeryService.GetFilteredListAsync(categoryId, minPrice, maxPrice);
        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories;
        ViewBag.SelectedCategoryId = categoryId;
        return View(items);
    }

    // GET: /Stationery/Detail/{id}
    public async Task<IActionResult> Detail(int id)
    {
        var item = await _stationeryService.GetDetailByIdAsync(id);
        if (item == null)
        {
            _logger.LogWarning("Details requested for non-existent item. ItemId={ItemId}", id);
            return NotFound();
        }
        return View(item);
    }

    // GET: /Stationery/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(cat => new SelectListItem
        {
            Value = cat.Id.ToString(),
            Text = cat.Name
        }).ToList();

        return View(new StationeryCreateViewModel());
    }

    // POST: /Stationery/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StationeryCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Custom business validation: SKU uniqueness check
            if (await _stationeryService.SkuExistsAsync(model.Sku))
            {
                ModelState.AddModelError(nameof(model.Sku), "Sku này đã tồn tại trong hệ thống (kể cả hàng đã xóa).");
            }
            else
            {
                await _stationeryService.CreateItemAsync(model);
                TempData["Success"] = "Đã thêm sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }
        }

        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(cat => new SelectListItem
        {
            Value = cat.Id.ToString(),
            Text = cat.Name
        }).ToList();

        return View(model);
    }

    // GET: /Stationery/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _stationeryService.GetEditDetailAsync(id);
        if (model == null)
        {
            _logger.LogWarning("Edit requested for non-existent item. ItemId={ItemId}", id);
            return NotFound();
        }

        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(cat => new SelectListItem
        {
            Value = cat.Id.ToString(),
            Text = cat.Name,
            Selected = cat.Id == model.CategoryId
        }).ToList();

        return View(model);
    }

    // POST: /Stationery/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StationeryEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            // Check SKU uniqueness excluding itself
            if (await _stationeryService.SkuExistsAsync(model.Sku, id))
            {
                ModelState.AddModelError(nameof(model.Sku), "Sku này đã tồn tại ở một sản phẩm khác.");
            }
            else
            {
                try
                {
                    await _stationeryService.UpdateItemAsync(model);
                    TempData["Success"] = "Đã cập nhật sản phẩm thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency conflict on update. ItemId={ItemId}", id);
                    ModelState.AddModelError(string.Empty, "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
        }

        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(cat => new SelectListItem
        {
            Value = cat.Id.ToString(),
            Text = cat.Name,
            Selected = cat.Id == model.CategoryId
        }).ToList();

        return View(model);
    }

    // GET: /Stationery/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _stationeryService.GetDetailByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    // POST: /Stationery/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _stationeryService.SoftDeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        TempData["Success"] = "Đã xóa mềm sản phẩm thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Stationery/Trash
    [HttpGet]
    public async Task<IActionResult> Trash()
    {
        var trashed = await _stationeryService.GetTrashedItemsAsync();
        return View(trashed);
    }

    // POST: /Stationery/Restore/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var restored = await _stationeryService.RestoreAsync(id);
        if (!restored)
        {
            return NotFound();
        }
        TempData["Success"] = "Đã khôi phục sản phẩm thành công.";
        return RedirectToAction(nameof(Trash));
    }

    // GET: /Stationery/Search
    [HttpGet]
    public async Task<IActionResult> Search()
    {
        var categories = await _stationeryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(cat => new SelectListItem
        {
            Value = cat.Id.ToString(),
            Text = cat.Name
        }).ToList();

        return View(new StationerySearchViewModel());
    }

    // GET/POST: /Stationery/PerformSearch (Dùng cho AJAX)
    public async Task<IActionResult> PerformSearch(string? keyword, int? categoryId, string? stockStatus)
    {
        var results = await _stationeryService.SearchItemsAsync(keyword, categoryId, stockStatus);
        return PartialView("_StationerySearchResultTable", results);
    }

    // GET: /Stationery/AdjustStock/{id}
    [HttpGet]
    public async Task<IActionResult> AdjustStock(int id)
    {
        var model = await _stationeryService.GetAdjustStockViewModelAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    // POST: /Stationery/AdjustStock/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(int id, StationeryAdjustStockViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _stationeryService.AdjustStockAsync(model);
                TempData["Success"] = $"Đã điều chỉnh tồn kho thành công cho sản phẩm {model.Sku}.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during stock adjustment. ItemId={ItemId}", id);
                ModelState.AddModelError(string.Empty, "Dữ liệu tồn kho hoặc phiên bản sản phẩm đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang.");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(model.AdjustmentQuantity), ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Re-fetch current quantity to show in UI
        var freshModel = await _stationeryService.GetAdjustStockViewModelAsync(id);
        if (freshModel != null)
        {
            model.CurrentQuantity = freshModel.CurrentQuantity;
        }

        return View(model);
    }
}
