using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Sample.Extensions.Interfaces;
using Sample.Observability;
using Sample.Telemetry;

namespace Sample
{
    internal class ApplicationInsightsAdapter : ICoreTelemetry
    {
        private readonly TelemetryClient telemetryClient;

        public ApplicationInsightsAdapter(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public ISpanActivity StartSpanActivity(string name)
        {
            return ApplicationInsightsSpanAdapter.StartSpan(this.telemetryClient, name);
        }

        private class ApplicationInsightsSpanAdapter : ISpanActivity
        {
            private readonly Activity activity;
            private TelemetryClient telemetryClient;

            private IOperationHolder<DependencyTelemetry> request;

            public static ApplicationInsightsSpanAdapter StartSpan(TelemetryClient telemetryClient, string name)
            {
                return new ApplicationInsightsSpanAdapter(telemetryClient, name);
            }

            private ApplicationInsightsSpanAdapter(TelemetryClient telemetryClient, string name)
            {
                this.activity = new Activity(name);

                if (Activity.Current != null)
                {
                    this.activity.SetParentId(Activity.Current.Id);
                }

                activity.Start();

                this.telemetryClient = telemetryClient;

                this.request = this.telemetryClient.StartOperation<DependencyTelemetry>(activity);
            }

            public void SetAttribute<T>(string key, T value)
            {
                this.request.Telemetry.Properties.Add(key, value.ToString());
            }

            public void Dispose()
            {
                this.request.Dispose();
                this.activity.Dispose();
            }
        }
    }
}
