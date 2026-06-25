using MiniStationeryManagement.Mvc.Models;
using MiniStationeryManagement.Mvc.Repositories;

namespace MiniStationeryManagement.Mvc.Services;

public class StationeryOrderService : IStationeryOrderService
{
    private readonly IStationeryOrderRepository _orderRepository;

    public StationeryOrderService(IStationeryOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task OrderStationeryAsync(string customerName, List<(int itemId, int qty)> items)
    {
        await _orderRepository.CreateOrderWithTransactionAsync(customerName, items);
    }

    public async Task<List<StationeryOrder>> GetOrderHistoryAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }
}
