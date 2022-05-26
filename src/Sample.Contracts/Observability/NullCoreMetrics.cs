using System;

namespace Sample.Observability
{
    public class NullCoreMetrics : ICoreMetrics
    {
        public void ApplicationInfo()
        {
        }

        public void OnException(Exception exception)
        {
        }

        public void OnHit(string path)
        {
        }
    }
}
