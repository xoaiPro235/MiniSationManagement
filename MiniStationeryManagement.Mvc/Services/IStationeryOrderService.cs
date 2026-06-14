namespace MiniStationeryManagement.Mvc.Services;

public interface IStationeryOrderService
{
    Task OrderStationeryAsync(string customerName, List<(int itemId, int qty)> items);
}
