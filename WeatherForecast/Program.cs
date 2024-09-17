using WeatherForecast.Models;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Services;
using WeatherForecast.Interfaces;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IWeatherRefreshServiceControl, WeatherRefreshService>();

builder.Services.AddSingleton<IWeatherStoringService>(provider =>
    new WeatherStoringService(
        () => provider.CreateScope().ServiceProvider.GetRequiredService<DataContext>()
    ));

builder.Services.AddHostedService<WeatherRefreshService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
