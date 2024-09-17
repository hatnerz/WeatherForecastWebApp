using WeatherForecast.Models;

namespace WeatherForecast.Interfaces
{
    public interface IWeatherStoringService
    {
        Task StoreWeatherDataAsync(WeatherData weatherData);
    }
}