using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Prometheus;
using Sample.Extensions;
using Sample.Extensions.Configurations;
using Sample.Extensions.Interfaces;
using Sample.Observability;
using Sample.Observability.Weather;
using Sample.Services;
using Sample.Services.Weather;
using Sample.Settings;
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
