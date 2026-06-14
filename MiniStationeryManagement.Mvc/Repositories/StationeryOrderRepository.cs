using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Models;

namespace MiniStationeryManagement.Mvc.Repositories;

public class StationeryOrderRepository : IStationeryOrderRepository
{
    private readonly AppDbContext _context;

    public StationeryOrderRepository(AppDbContext context)
    {
        _context = context;
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

            await transaction.CommitAsync();
            return order;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
