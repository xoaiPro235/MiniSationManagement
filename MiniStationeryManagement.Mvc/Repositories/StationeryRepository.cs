using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Models;

namespace MiniStationeryManagement.Mvc.Repositories;

public class StationeryRepository : IStationeryRepository
{
    private readonly AppDbContext _context;

    public StationeryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StationeryItem>> GetAllAsync()
    {
        return await _context.StationeryItems
            .Include(s => s.Category)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<StationeryItem?> GetByIdAsync(int id)
    {
        return await _context.StationeryItems
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<StationeryItem?> GetByIdIgnoreFilterAsync(int id)
    {
        return await _context.StationeryItems
            .IgnoreQueryFilters()
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<StationeryItem>> GetTrashedAsync()
    {
        return await _context.StationeryItems
            .IgnoreQueryFilters()
            .Where(s => s.IsDeleted)
            .Include(s => s.Category)
            .OrderByDescending(s => s.DeletedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<StationeryItem>> GetFilteredAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    )
    {
        var query = _context.StationeryItems
            .Include(s => s.Category)
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(s => s.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(s => s.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(s => s.Price <= maxPrice.Value);

        return await query.ToListAsync();
    }

    public async Task UpdateStockAsync(int itemId, int quantity)
    {
        var item = await _context.StationeryItems.FindAsync(itemId);
        if (item != null)
        {
            item.Quantity += quantity;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<StationeryCategory>> GetAllCategoriesAsync()
    {
        return await _context.StationeryCategories.ToListAsync();
    }

    public async Task<List<StationeryCategory>> GetCategoriesWithItemsAsync()
    {
        return await _context.StationeryCategories
            .Include(c => c.StationeryItems)
            .ToListAsync();
    }

    public async Task AddAsync(StationeryItem item)
    {
        await _context.StationeryItems.AddAsync(item);
    }

    public async Task UpdateAsync(StationeryItem item, uint originalRowVersion)
    {
        _context.Entry(item).Property(s => s.RowVersion).OriginalValue = originalRowVersion;
        _context.StationeryItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<StationeryItem>> SearchAsync(string? keyword, int? categoryId, string? stockStatus)
    {
        var query = _context.StationeryItems
            .Include(s => s.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(lowerKeyword) || s.Sku.ToLower().Contains(lowerKeyword));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(stockStatus))
        {
            if (stockStatus.Equals("low", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Quantity <= s.MinStock && s.Quantity > 0);
            }
            else if (stockStatus.Equals("out", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Quantity == 0);
            }
            else if (stockStatus.Equals("normal", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Quantity > s.MinStock);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<bool> SkuExistsIgnoreFiltersAsync(string sku, int? excludeId = null)
    {
        var query = _context.StationeryItems.IgnoreQueryFilters().AsNoTracking();
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        return await query.AnyAsync(s => s.Sku.ToLower() == sku.ToLower());
    }
}
