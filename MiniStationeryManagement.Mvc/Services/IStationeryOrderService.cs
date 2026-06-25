using System.Collections.Generic;
using System.Threading.Tasks;
using MiniStationeryManagement.Mvc.Models;

namespace MiniStationeryManagement.Mvc.Services;

public interface IStationeryOrderService
{
    Task OrderStationeryAsync(string customerName, List<(int itemId, int qty)> items);
    Task<List<StationeryOrder>> GetOrderHistoryAsync();
}
