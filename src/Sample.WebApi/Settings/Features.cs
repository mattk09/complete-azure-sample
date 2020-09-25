namespace Sample.Settings
{
    public class Features
    {
        public bool UseStorageSimulator { get; set; } = true;

        public TelemetrySdk Telemetry { get; set; } = TelemetrySdk.None;
    }
}