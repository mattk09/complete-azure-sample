using OpenTelemetry.Trace;
using Sample.Telemetry;

namespace Sample
{
    internal class OpenTelemetryAdapter : ICoreTelemetry
    {
        private readonly Tracer tracer;

        public OpenTelemetryAdapter(TracerProvider tracerProvider)
        {
            // this.tracer = tracerProvider.GetTracer(nameof(OpenTelemetryAdapter)); "Samples.SampleServer"
            // TODO: Source must be added in the config, how to abstract this away?
            this.tracer = tracerProvider.GetTracer("Samples.SampleServer");
        }

        public ISpanActivity StartSpanActivity(string name)
        {
            return OpenTelemetrySpanAdapter.StartSpan(this.tracer, name);
        }

        private class OpenTelemetrySpanAdapter : ISpanActivity
        {
            private TelemetrySpan Span { get; set; }

            public static OpenTelemetrySpanAdapter StartSpan(Tracer tracer, string name)
            {
                return new OpenTelemetrySpanAdapter(tracer, name);
            }

            private OpenTelemetrySpanAdapter(Tracer tracer, string name)
            {
                var activity = System.Diagnostics.Activity.Current;
                var current = Tracer.CurrentSpan;

                // this.Span = tracer.StartSpan(name, kind: SpanKind.Internal, parentSpan: current);
                this.Span = tracer.StartActiveSpan(name);
            }

            public void SetAttribute<T>(string key, T value)
            {
                this.Span.SetAttribute(key, value.ToString());
            }

            public void Dispose()
            {
                this.Span.Dispose();
            }
        }
    }
}