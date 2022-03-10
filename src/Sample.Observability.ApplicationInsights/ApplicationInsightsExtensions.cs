using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Observability
{
    public static class ApplicationInsightsExtensions
    {
        public static IServiceCollection AddCoreTracing(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseSomeMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder;
        }
    }
}