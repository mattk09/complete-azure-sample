using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Observability;
using Sample.Observability.Settings;

[assembly: FunctionsStartup(typeof(Sample.Functions.Startup))]

namespace Sample.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            /*
            base.ConfigureAppConfiguration(builder);

            var builtConfig = builder.ConfigurationBuilder.Build();
            var keyVaultName = builtConfig["KeyVaultNameFromDeployment"];

            if (!string.IsNullOrEmpty(keyVaultName))
            {
                builder.ConfigurationBuilder.AddAzureKeyVault(
                    new Uri($"https://{keyVaultName}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
            */
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.GetContext().Configuration;

            services.AddCoreMetrics();

            string telemetryProvider = configuration.GetValue<string>("TelemetryProvider", "None");

            services = telemetryProvider switch
            {
                "ApplicationInsights" => services.AddApplicationInsightsCoreTelemetry(builder.GetContext().Configuration),
                "OpenTelemetry" => services.AddOpenCoreTelemetry(configuration.GetSection("OpenTelemetrySettings").Get<OpenTelemetrySettings>()),
                _ => services.AddSingleton<ICoreTelemetry, NullCoreTelemetry>(),
            };
        }
    }
}