using Microsoft.EntityFrameworkCore;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public class WeatherStoringService : IWeatherStoringService
    {
        private readonly Func<DataContext> dataContextFactory;

        public WeatherStoringService(Func<DataContext> dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public async Task StoreWeatherDataAsync(WeatherData weatherData)
        {
            using (var context = dataContextFactory())
            {
                if (weatherData == null)
                    throw new ArgumentNullException(nameof(weatherData));

                var existingData = await context.WeatherData
                    .FirstOrDefaultAsync(wd => wd.City == weatherData.City && wd.ZipCode == weatherData.ZipCode);

                if (existingData != null)
                {
                    existingData.Temperature = weatherData.Temperature;
                    existingData.Description = weatherData.Description;
                    existingData.LastUpdated = DateTime.Now;
                }
                else
                {
                    context.WeatherData.Add(weatherData);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
