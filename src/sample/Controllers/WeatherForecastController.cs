using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            using (logger.BeginScope(new Dictionary<string, object> { { "SampleKey", "SampleValue" } }))
            {
                var random = new Random();

                var forecast = Enumerable.Range(1, 5)
                    .Select(
                        index => new WeatherForecast
                        {
                            Date = DateTime.Now.AddDays(index),
                            TemperatureC = random.Next(-20, 55),
                            Summary = Summaries[random.Next(Summaries.Length)],
                        })
                    .ToList();

                logger.LogInformation($"WeatherForecast Get called.  Returning {forecast.Count} entries");

                return forecast;
            }
        }
    }
}
