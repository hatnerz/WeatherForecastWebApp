using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Exceptions;
using WeatherForecast.Interfaces;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService weatherService;
        private readonly IWeatherRefreshServiceControl weatherRefreshService;
        private readonly IWeatherStoringService weatherStoringService;

        public WeatherController(IWeatherService weatherService, IWeatherRefreshServiceControl weatherRefreshService, IWeatherStoringService weatherStoringService)
        {
            this.weatherService = weatherService;
            this.weatherRefreshService = weatherRefreshService;
            this.weatherStoringService = weatherStoringService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCity(string cityName)
        {
            try
            {
                var weatherData = await weatherService.GetWeatherByCityNameAsync(cityName);
                weatherRefreshService.AddLocationIfNotExist(weatherData.City);
                await weatherStoringService.StoreWeatherDataAsync(weatherData);
                return View("WeatherDetails", weatherData);
            }
            catch (WeatherException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchByZipCode(string zipCode)
        {
            try
            {
                var weatherData = await weatherService.GetWeatherByZipCodeAsync(zipCode);
                weatherRefreshService.AddLocationIfNotExist(weatherData.City);
                await weatherStoringService.StoreWeatherDataAsync(weatherData);
                return View("WeatherDetails", weatherData);
            }
            catch (WeatherException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }
        }
    }
}
