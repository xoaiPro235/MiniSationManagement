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
        return await _context.StationeryItems.Include(s => s.Category).ToListAsync();
    }

    public async Task<StationeryItem?> GetByIdAsync(int id)
    {
        return await _context
            .StationeryItems.Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<StationeryItem>> GetFilteredAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    )
    {
        var query = _context.StationeryItems.Include(s => s.Category).AsNoTracking().AsQueryable();

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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<StationeryItem>> SearchAsync(string? keyword, int? categoryId)
    {
        var query = _context.StationeryItems.Include(s => s.Category).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.Name.Contains(keyword) || s.Sku.Contains(keyword));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        return await query.ToListAsync();
    }
}
