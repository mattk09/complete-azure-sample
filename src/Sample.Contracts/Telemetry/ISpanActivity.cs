using System;

namespace Sample.Telemetry
{
    public interface ISpanActivity : IDisposable
    {
        void SetAttribute<T>(string key, T value);
    }
}
