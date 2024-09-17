using System.Threading.Tasks;
using WeatherForecast.Models;

namespace WeatherForecast.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherByCityNameAsync(string cityName);
        Task<WeatherData> GetWeatherByZipCodeAsync(string zipCode);
    }
}