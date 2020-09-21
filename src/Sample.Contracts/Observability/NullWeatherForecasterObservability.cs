using System;

namespace Sample.Observability
{
    public class NullWeatherForecasterObservability
    {
        public IDisposable GetForecast()
        {
            return new NullDisposable();
        }

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
