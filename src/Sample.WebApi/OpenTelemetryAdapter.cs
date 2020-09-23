using OpenTelemetry.Trace;
using Sample.Telemetry;

namespace Sample
{
    internal class OpenTelemetryAdapter : ICoreTelemetry
    {
        private readonly Tracer tracer;

        public OpenTelemetryAdapter(TracerProvider tracerProvider)
        {
            this.tracer = tracerProvider.GetTracer(nameof(OpenTelemetryAdapter));
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
                return new OpenTelemetrySpanAdapter() { Span = tracer.StartSpan(name, SpanKind.Internal, Tracer.CurrentSpan) };
            }

            private OpenTelemetrySpanAdapter()
            {
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