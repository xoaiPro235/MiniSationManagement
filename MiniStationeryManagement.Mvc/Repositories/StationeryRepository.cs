namespace MiniStationeryManagement.Mvc.Repositories;

using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Models;

public class StationeryRepository : IStationeryRepository
{
    private AppDbContext _context;

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

    public async Task<StationeryOrder> CreateOrderWithTransactionAsync(
        string customerName,
        List<(int itemId, int qty)> items
    )
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new StationeryOrder
            {
                OrderDate = DateTime.UtcNow,
                CustomerName = customerName,
                TotalAmount = 0,
            };
            _context.StationeryOrders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;

            foreach (var itemDetail in items)
            {
                var item = await _context.StationeryItems.FindAsync(itemDetail.itemId);
                if (item == null)
                    throw new Exception($"Sản phẩm mã số {itemDetail.itemId} không tồn tại.");

                if (item.Quantity < itemDetail.qty)
                    throw new InvalidOperationException(
                        $"Sản phẩm '{item.Name}' không đủ số lượng trong kho (Hiện còn: {item.Quantity})."
                    );

                // Trừ tồn kho sản phẩm
                item.Quantity -= itemDetail.qty;

                var orderItem = new StationeryOrderItem
                {
                    OrderId = order.Id,
                    StationeryItemId = item.Id,
                    Quantity = itemDetail.qty,
                    UnitPrice = item.Price,
                };
                _context.StationeryOrderItems.Add(orderItem);
                total += item.Price * itemDetail.qty;
            }

            order.TotalAmount = total;
            await _context.SaveChangesAsync();

            // Nếu mọi thứ thành công thực hiện Commit
            await transaction.CommitAsync();
            return order;
        }
        catch (Exception)
        {
            // Nếu có bất kì lỗi nào xảy ra hệ thống tự động Rollback lại toàn bộ dữ liệu ban đầu
            await transaction.RollbackAsync();
            throw;
        }
    }
}
