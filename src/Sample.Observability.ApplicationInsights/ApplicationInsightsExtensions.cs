using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Observability
{
    public static class ApplicationInsightsExtensions
    {
        public static IServiceCollection AddApplicationInsightsCoreTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            // By default this will look for 'ApplicationInsights:InstrumentationKey' in the configuration.
            services.AddApplicationInsightsTelemetry(configuration);
            services.AddSingleton<ICoreTelemetry, ApplicationInsightsAdapter>();

            return services;
        }
    }
}