using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniStationeryManagement.Mvc.ViewModels;

namespace MiniStationeryManagement.Mvc.Controllers;

public class AuditLogsController : Controller
{
    private readonly IWebHostEnvironment _env;

    public AuditLogsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // GET: /AuditLogs
    public IActionResult Index(string? date, string? level)
    {
        var logsDir = Path.Combine(_env.ContentRootPath, "logs");
        var availableDates = new List<string>();
        var logs = new List<AuditLogViewModel>();

        // 1. Get available log files and extract dates
        if (Directory.Exists(logsDir))
        {
            var files = Directory.GetFiles(logsDir, "lab05-*.txt");
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                // Matches lab05-yyyyMMdd.txt or similar
                var dateMatch = Regex.Match(fileName, @"lab05-(\d{8})\.txt");
                if (dateMatch.Success)
                {
                    var dateStr = dateMatch.Groups[1].Value; // yyyyMMdd
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                    {
                        availableDates.Add(dt.ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    // Fallback matching for lab05-.txt (today's active file)
                    if (fileName == "lab05-.txt")
                    {
                        var writeTime = System.IO.File.GetLastWriteTime(file);
                        availableDates.Add(writeTime.ToString("yyyy-MM-dd"));
                    }
                }
            }
        }

        // Sort dates descending
        availableDates = availableDates.Distinct().OrderByDescending(d => d).ToList();
        ViewBag.AvailableDates = availableDates.Select(d => new SelectListItem { Value = d, Text = d, Selected = d == date }).ToList();

        // 2. Select default date if none specified
        string selectedDate = date ?? availableDates.FirstOrDefault() ?? DateTime.Today.ToString("yyyy-MM-dd");

        // 3. Locate the corresponding log file
        string? targetFilePath = null;
        if (DateTime.TryParse(selectedDate, out var selectedDt))
        {
            var dateSuffix = selectedDt.ToString("yyyyMMdd");
            var file1 = Path.Combine(logsDir, $"lab05-{dateSuffix}.txt");
            var file2 = Path.Combine(logsDir, "lab05-.txt"); // Serilog active write file

            if (System.IO.File.Exists(file1))
            {
                targetFilePath = file1;
            }
            else if (System.IO.File.Exists(file2) && selectedDt.Date == DateTime.Today)
            {
                targetFilePath = file2;
            }
        }

        // 4. Parse the log file
        if (targetFilePath != null)
        {
            logs = ParseLogFile(targetFilePath);
        }

        // 5. Filter by log level
        if (!string.IsNullOrEmpty(level) && level != "ALL")
        {
            logs = logs.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Sort logs descending (newest first)
        logs = logs.OrderByDescending(l => l.Timestamp).ToList();

        ViewBag.SelectedLevel = level ?? "ALL";
        ViewBag.SelectedDate = selectedDate;

        return View(logs);
    }

    private List<AuditLogViewModel> ParseLogFile(string filePath)
    {
        var logs = new List<AuditLogViewModel>();
        if (!System.IO.File.Exists(filePath)) return logs;

        try
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Regex for Serilog File formatting:
                    // 2026-06-27 20:49:31.123 +07:00 [INF] Stationery item created successfully...
                    var match = Regex.Match(line, @"^(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{3}\s[+-]\d{2}:\d{2})\s\[([A-Z]{3})\]\s(.*)$");
                    if (match.Success)
                    {
                        var tsString = match.Groups[1].Value;
                        var lvl = match.Groups[2].Value;
                        var msg = match.Groups[3].Value;

                        DateTime.TryParseExact(tsString, "yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ts);
                        if (ts == default)
                        {
                            DateTime.TryParse(tsString, out ts);
                        }

                        var itemIdMatch = Regex.Match(msg, @"ItemId=(\d+)");
                        var skuMatch = Regex.Match(msg, @"Sku=([A-Z0-9\-]+)");

                        logs.Add(new AuditLogViewModel
                        {
                            Timestamp = ts == default ? DateTime.Now : ts,
                            Level = lvl,
                            Message = msg,
                            ProductId = itemIdMatch.Success ? itemIdMatch.Groups[1].Value : string.Empty,
                            SKU = skuMatch.Success ? skuMatch.Groups[1].Value : string.Empty
                        });
                    }
                    else
                    {
                        // Fallback parsing
                        var altMatch = Regex.Match(line, @"^\[(\d{2}:\d{2}:\d{2})\s([A-Z]{3})\]\s(.*)$");
                        if (altMatch.Success)
                        {
                            var timeStr = altMatch.Groups[1].Value;
                            var lvl = altMatch.Groups[2].Value;
                            var msg = altMatch.Groups[3].Value;

                            DateTime.TryParse(timeStr, out var ts);

                            var itemIdMatch = Regex.Match(msg, @"ItemId=(\d+)");
                            var skuMatch = Regex.Match(msg, @"Sku=([A-Z0-9\-]+)");

                            logs.Add(new AuditLogViewModel
                            {
                                Timestamp = ts == default ? DateTime.Now : ts,
                                Level = lvl,
                                Message = msg,
                                ProductId = itemIdMatch.Success ? itemIdMatch.Groups[1].Value : string.Empty,
                                SKU = skuMatch.Success ? skuMatch.Groups[1].Value : string.Empty
                            });
                        }
                        else if (!string.IsNullOrWhiteSpace(line))
                        {
                            // Raw line
                            logs.Add(new AuditLogViewModel
                            {
                                Timestamp = DateTime.Now,
                                Level = "RAW",
                                Message = line
                            });
                        }
                    }
                }
            }
        }
        catch (IOException)
        {
            // Log file is being written or locked, return whatever we have
        }

        return logs;
    }
}
