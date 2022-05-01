using Sample.Observability.Settings;
using Sample.Storage.Azure.Settings;

namespace Sample.Settings
{
    public class SampleSettings
    {
        public Features Features { get; set; } = new Features();

        public AzureStorageSettings AzureStorageSettings { get; set; } = new AzureStorageSettings();

        public OpenTelemetrySettings OpenTelemetrySettings { get; set; } = new OpenTelemetrySettings();

        public TelemetryProvider TelemetryProvider { get; set; } = TelemetryProvider.None;
    }
}