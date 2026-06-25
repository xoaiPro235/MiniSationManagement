using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Controllers;

public class DataHealthController : Controller
{
    private readonly IDataHealthService _healthService;

    public DataHealthController(IDataHealthService healthService)
    {
        _healthService = healthService;
    }

    // GET: /DataHealth
    public async Task<IActionResult> Index()
    {
        var model = await _healthService.CheckHealthAsync();
        return View(model);
    }
}
