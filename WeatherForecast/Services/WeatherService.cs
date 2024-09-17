using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using WeatherForecast.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using WeatherForecast.Exceptions;
using WeatherForecast.Interfaces;

namespace WeatherForecast.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private readonly string weatherUrl = "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.apiKey = configuration["WeatherApi:ApiKey"];
        }

        public async Task<WeatherData> GetWeatherByCityNameAsync(string cityName)
        {
            var url = $"{weatherUrl}?q={cityName}&appid={apiKey}&units=metric";
            return await GetWeatherDataAsync(url);
        }

        public async Task<WeatherData> GetWeatherByZipCodeAsync(string zipCode)
        {
            var url = $"{weatherUrl}?zip={zipCode}&appid={apiKey}&units=metric";
            return await GetWeatherDataAsync(url);
        }

        private async Task<WeatherData> GetWeatherDataAsync(string url)
        {
            var response = await httpClient.GetAsync(url);

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                throw new WeatherException(errorResponse.Message);
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content);

            return new WeatherData
            {
                City = weatherApiResponse.Name,
                ZipCode = "", // This needs to be handled based on input
                Temperature = weatherApiResponse.Main.Temp,
                Description = weatherApiResponse.Weather[0].Description,
                LastUpdated = DateTime.Now
            };
        }

        private class WeatherApiResponse
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("main")]
            public Main Main { get; set; }

            [JsonPropertyName("weather")]
            public Weather[] Weather { get; set; }
        }

        private class Main
        {
            [JsonPropertyName("temp")]
            public double Temp { get; set; }
        }

        private class Weather
        {
            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        private class ErrorResponse
        {
            [JsonPropertyName("cod")]
            public string Code { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}