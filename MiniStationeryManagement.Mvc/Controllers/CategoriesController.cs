using Microsoft.AspNetCore.Mvc;
using MiniStationeryManagement.Mvc.Services;
using System.Threading.Tasks;

namespace MiniStationeryManagement.Mvc.Controllers;

[Route("Categories")]
[Route("Category")]
public class CategoriesController : Controller
{
    private readonly IStationeryService _stationeryService;

    public CategoriesController(IStationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var relationships = await _stationeryService.GetCategoryRelationshipsAsync();
        return View(relationships);
    }
}
