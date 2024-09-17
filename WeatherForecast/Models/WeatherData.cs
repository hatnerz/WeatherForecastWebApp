using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Models
{
    public class WeatherData
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
