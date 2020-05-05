using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.Core.Models;

namespace Sample.Core
{
    public class WeatherForecaster : IWeatherForecaster
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        public Task<IEnumerable<WeatherForecast>> GetForecastAsync()
        {
            var random = new Random();

            var forecast = Enumerable.Range(1, 5)
                .Select(
                    index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = random.Next(-20, 55),
                        Summary = Summaries[random.Next(Summaries.Length)],
                    });

            return Task.FromResult(forecast);
        }
    }
}
