using Microsoft.EntityFrameworkCore;

namespace WeatherForecast.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<WeatherData> WeatherData { get; set; }
    }
}
