using System;

namespace Sample.Telemetry
{
    public class NullCoreTelemetry : ICoreTelemetry
    {
        private static readonly ISpanActivity DefaultSpanActivity = new NullSpanActivity();

        public ISpanActivity StartSpanActivity(string name)
        {
            return DefaultSpanActivity;
        }

        private class NullSpanActivity : ISpanActivity
        {
            public void SetAttribute<T>(string key, T value)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
