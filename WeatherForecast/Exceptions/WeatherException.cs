namespace WeatherForecast.Exceptions
{
    public class WeatherException : Exception
    {
        public WeatherException(string message) : base(message) { }
    }
}
