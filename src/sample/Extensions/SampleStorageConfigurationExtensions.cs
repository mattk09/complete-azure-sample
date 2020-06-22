using System;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Storage;
using Sample.Settings;
using Sample.Storage;
using Sample.Storage.Settings;

namespace Sample.Extensions
{
    public static class SampleStorageConfigurationExtensions
    {
        public static void AddSampleStorage(this IServiceCollection services, SampleSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.Features.UseStorageSimulator)
            {
                services.AddSingleton<IStorage, MemoryStorage>();
            }
            else
            {
                services.AddSingleton<AzureStorageSettings>(settings.AzureStorageSettings);
                services.AddSingleton<IStorage, AzureBlobStorage>();
            }
        }
    }
}