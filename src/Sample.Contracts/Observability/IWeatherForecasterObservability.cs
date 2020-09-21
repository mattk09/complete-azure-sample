using System;

namespace Sample.Observability
{
    public interface IWeatherForecasterObservability
    {
        IDisposable GetForecast();
    }
}
