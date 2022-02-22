using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;

[assembly: FunctionsStartup(typeof(Sample.Functions.Startup))]

namespace Sample.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            var builtConfig = context.Configuration;
            var keyVaultName = builtConfig["KeyVaultNameFromDeployment"];

            if (!string.IsNullOrEmpty(keyVaultName))
            {
                builder.ConfigurationBuilder.AddAzureKeyVault(
                    new Uri($"https://{keyVaultName}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}