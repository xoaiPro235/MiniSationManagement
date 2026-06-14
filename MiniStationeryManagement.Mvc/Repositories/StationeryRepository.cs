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
}
