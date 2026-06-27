using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Repositories;

public class DataHealthRepository : IDataHealthRepository
{
    private readonly AppDbContext _context;

    public DataHealthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetPendingMigrationsAsync()
    {
        var pending = await _context.Database.GetPendingMigrationsAsync();
        return pending.ToList();
    }

    public async Task<bool> HasCategoriesAsync()
    {
        return await _context.StationeryCategories.AnyAsync();
    }

    public async Task<bool> HasItemsAsync()
    {
        return await _context.StationeryItems.AnyAsync();
    }

    public async Task<bool> TestNoTrackingAsync()
    {
        var item = await _context.StationeryItems.AsNoTracking().FirstOrDefaultAsync();
        return true;
    }

    public async Task<bool> TestTransactionAsync()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var count = await _context.StationeryItems.CountAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}
