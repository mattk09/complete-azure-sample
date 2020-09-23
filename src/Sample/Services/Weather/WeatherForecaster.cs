using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.Models;
using Sample.Observability.Weather;
using Sample.Services;

namespace Sample.Services.Weather
{
    public class WeatherForecaster : IWeatherForecaster
    {
        private static readonly string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly IWeatherForecasterObservability observability;

        public WeatherForecaster(IWeatherForecasterObservability observability)
        {
            this.observability = observability;
        }

        public Task<IEnumerable<WeatherForecast>> GetForecastAsync()
        {
            using var activity = this.observability.GetForecast();
            var random = new Random();

            var forecast = Enumerable.Range(1, 5)
                .Select(
                    index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = random.Next(-20, 55),
                        Summary = summaries[random.Next(summaries.Length)],
                    });

            return Task.FromResult(forecast);
        }
    }
}
