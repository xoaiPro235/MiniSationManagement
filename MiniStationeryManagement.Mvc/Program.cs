using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Data;
using MiniStationeryManagement.Mvc.Options;
using MiniStationeryManagement.Mvc.Repositories;
using MiniStationeryManagement.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IStationeryRepository, StationeryRepository>();
builder.Services.AddScoped<StationeryService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Stationery}/{action=Index}/{id?}");

app.Run();
