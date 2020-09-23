using System;

namespace Sample.Observability.Weather
{
    public interface IWeatherForecasterObservability
    {
        IDisposable GetForecast();
    }
}
