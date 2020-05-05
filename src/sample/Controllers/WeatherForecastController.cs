using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.Core;
using Sample.Core.Models;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecaster weatherForecaster;
        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(IWeatherForecaster weatherForecaster, ILogger<WeatherForecastController> logger)
        {
            this.weatherForecaster = weatherForecaster;
            this.logger = logger;
        }

        [HttpGet]
        public Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            using (this.logger.BeginScope(new Dictionary<string, object> { { "SampleKey", "SampleValue" } }))
            {
                return this.weatherForecaster.GetForecastAsync();
            }
        }
    }
}
