namespace WeatherForecast.Interfaces
{
    public interface IWeatherRefreshServiceControl
    {
        void AddLocationIfNotExist(string location);
    }
}
