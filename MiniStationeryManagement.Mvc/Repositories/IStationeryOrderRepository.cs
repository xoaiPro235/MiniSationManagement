using MiniStationeryManagement.Mvc.Models;

namespace MiniStationeryManagement.Mvc.Repositories;

public interface IStationeryOrderRepository
{
    Task<StationeryOrder> CreateOrderWithTransactionAsync(
        string customerName,
        List<(int itemId, int qty)> items
    );
}
