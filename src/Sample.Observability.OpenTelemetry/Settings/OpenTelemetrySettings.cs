using System.Collections.Generic;

namespace Sample.Observability.Settings
{
    public class OpenTelemetrySettings
    {
        public string ServiceName { get; set; } = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

        public string ServiceVersion { get; set; } = "1.0";

        public string JaegerExporterHost { get; set; }

        public int JaegerExporterPort { get; set; } = 6831;

        public IEnumerable<string> Sources { get; set; } = System.Array.Empty<string>();
    }
}