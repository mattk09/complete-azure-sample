using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Core.Models;

namespace Sample.Core
{
    public interface IWeatherForecaster
    {
        Task<IEnumerable<WeatherForecast>> GetForecastAsync();
    }
}
