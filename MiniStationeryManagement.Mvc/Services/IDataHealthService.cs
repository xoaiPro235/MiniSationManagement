using MiniStationeryManagement.Mvc.ViewModels;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Services;

public interface IDataHealthService
{
    Task<DataHealthViewModel> CheckHealthAsync();
}
