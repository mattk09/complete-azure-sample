using System;
using Prometheus;

namespace Sample.Observability
{
    internal class CoreMetrics : ICoreMetrics
    {
        private readonly Gauge applicationInfoCounter = Metrics.CreateGauge(
            "app_info",
            "Basic application runtime information",
            "version", "description");

        private readonly Counter totalExceptions = Metrics.CreateCounter(
            "sample_exceptions_total",
            "The total number of exceptions encountered.",
            "exception_type");

        private readonly Counter totalApiHits = Metrics.CreateCounter(
            "sample_api_hits_total",
            "The total number of requests serviced.",
            "path");

        public void ApplicationInfo()
        {
            this.applicationInfoCounter
                .WithLabels(System.Environment.Version.ToString(), System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription)
                .Set(1);
        }

        public void OnException(Exception exception)
        {
            this.totalExceptions
                .WithLabels(exception.GetType().Name)
                .Inc();
        }

        public void OnHit(string path)
        {
            this.totalApiHits
                .WithLabels(path)
                .Inc();
        }
    }
}