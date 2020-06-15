using Sample.Storage.Settings;

namespace Sample.Settings
{
    public class SampleSettings
    {
        public Features Features { get; set; } = new Features();

        public AzureStorageSettings AzureStorageSettings { get; set; } = new AzureStorageSettings();
    }
}