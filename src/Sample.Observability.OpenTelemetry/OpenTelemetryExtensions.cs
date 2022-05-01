using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Sample.Observability
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenCoreTelemetry(this IServiceCollection services, string serviceName, string serviceVersion, params string[] sources)
        {
            services.AddSingleton<ICoreTelemetry, OpenTelemetryAdapter>();

            return services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .AddSource(serviceName, nameof(OpenTelemetryAdapter))
                    .AddSource(sources)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(jaegerOptions =>
                        {
                            jaegerOptions.AgentHost = "jaeger"; // Use name from docker-compose file, not "localhost";
                            jaegerOptions.AgentPort = 6831;
                        });
            });
        }
    }
}