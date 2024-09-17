using System.Collections.Concurrent;
using WeatherForecast.Interfaces;

namespace WeatherForecast.Services
{
    public class WeatherRefreshService : IHostedService, IDisposable, IWeatherRefreshServiceControl
    {
        private readonly IWeatherService _weatherService;
        private readonly IWeatherStoringService _weatherStoringService;
        private Timer _timer;
        private readonly ILogger<WeatherRefreshService> _logger;
        private TimeSpan refreshInterval { get; set; } = TimeSpan.FromHours(1);
        private ConcurrentBag<string> _locationsToRefresh;

        public WeatherRefreshService(IConfiguration configuration, IWeatherService weatherService, IWeatherStoringService weatherStoringService, ILogger<WeatherRefreshService>? logger)
        {
            int refreshIntervalInMinutes = configuration.GetValue<int>("WeatherService:RefreshIntervalInMinutes");
            if (refreshIntervalInMinutes > 0) 
            {
                refreshInterval = TimeSpan.FromMinutes(refreshIntervalInMinutes);
            }

            _weatherService = weatherService;
            _weatherStoringService = weatherStoringService;
            _logger = logger;
            _locationsToRefresh = new ConcurrentBag<string>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RefreshWeatherData, null, TimeSpan.Zero, this.refreshInterval);
            return Task.CompletedTask;
        }

        private async void RefreshWeatherData(object state)
        {
            foreach (var location in _locationsToRefresh)
            {
                try
                {
                    var weatherData = await _weatherService.GetWeatherByCityNameAsync(location);
                    await _weatherStoringService.StoreWeatherDataAsync(weatherData);
                    _logger.LogInformation($"Successfully refreshed weather data for {location}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error refreshing weather data for {location}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void AddLocationIfNotExist(string location)
        {
            if(!_locationsToRefresh.Contains(location))
            {
                _locationsToRefresh.Add(location);
            }
        }
    }
}
