using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Repositories;

public interface IDataHealthRepository
{
    Task<List<string>> GetPendingMigrationsAsync();
    Task<bool> HasCategoriesAsync();
    Task<bool> HasItemsAsync();
    Task<bool> TestNoTrackingAsync();
    Task<bool> TestTransactionAsync();
}
