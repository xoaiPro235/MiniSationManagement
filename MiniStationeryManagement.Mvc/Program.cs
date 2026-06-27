using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Options;
using MiniStationeryManagement.Mvc.Repositories;
using MiniStationeryManagement.Mvc.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/lab05-.txt", rollingInterval: RollingInterval.Day));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IStationeryRepository, StationeryRepository>();
builder.Services.AddScoped<IStationeryService, StationeryService>();
builder.Services.AddScoped<IStationeryOrderRepository, StationeryOrderRepository>();
builder.Services.AddScoped<IStationeryOrderService, StationeryOrderService>();
builder.Services.AddScoped<IDataHealthRepository, DataHealthRepository>();
builder.Services.AddScoped<IDataHealthService, DataHealthService>();

builder.Services.AddControllersWithViews();

// Configure ProblemDetails
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    };
});

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running."), tags: new[] { "live" })
    .AddDbContextCheck<AppDbContext>("database", tags: new[] { "ready" });

var app = builder.Build();

// Configure Exception Handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Health Checks
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description ?? "No description",
                duration = entry.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

// Map API Endpoint with ProblemDetails
app.MapGet("/api/stationery/{id:int}", async (int id, AppDbContext db, HttpContext http) =>
{
    var item = await db.StationeryItems.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    if (item == null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://example.com/problems/stationery-not-found",
            Title = "Stationery Not Found",
            Detail = $"The stationery item with ID {id} was not found.",
            Status = StatusCodes.Status404NotFound,
            Instance = http.Request.Path
        };
        problemDetails.Extensions["errorCode"] = "STATIONERY_NOT_FOUND";
        
        return Results.Problem(problemDetails);
    }

    return Results.Ok(item);
});

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
