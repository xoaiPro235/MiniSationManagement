using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Models;

namespace MiniStationeryManagement.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public HomeController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Fetch live database metrics using AsNoTracking
        var total = await _context.StationeryItems.IgnoreQueryFilters().AsNoTracking().CountAsync();
        var active = await _context.StationeryItems.AsNoTracking().CountAsync();
        var deleted = await _context.StationeryItems.IgnoreQueryFilters().AsNoTracking().CountAsync(s => s.IsDeleted);
        var lowStock = await _context.StationeryItems.AsNoTracking().CountAsync(s => s.Quantity <= s.MinStock);

        // 2. Count today's logs from Serilog
        int logsToday = 0;
        var logsDir = Path.Combine(_env.ContentRootPath, "logs");
        var todaySuffix = DateTime.Today.ToString("yyyyMMdd");
        var logFile1 = Path.Combine(logsDir, $"lab05-{todaySuffix}.txt");
        var logFile2 = Path.Combine(logsDir, "lab05-.txt");

        string? activeLogFile = null;
        if (System.IO.File.Exists(logFile1))
        {
            activeLogFile = logFile1;
        }
        else if (System.IO.File.Exists(logFile2))
        {
            activeLogFile = logFile2;
        }

        if (activeLogFile != null)
        {
            try
            {
                using (var fs = new FileStream(activeLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    while (reader.ReadLine() != null)
                    {
                        logsToday++;
                    }
                }
            }
            catch (IOException)
            {
                // Silently ignore or set fallback
            }
        }

        ViewBag.TotalCount = total;
        ViewBag.ActiveCount = active;
        ViewBag.DeletedCount = deleted;
        ViewBag.LowStockCount = lowStock;
        ViewBag.LogsCount = logsToday;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }

    [Route("Home/StatusCode")]
    public new IActionResult StatusCode(int code)
    {
        ViewBag.StatusCode = code;
        return View();
    }
}
