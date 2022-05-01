using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample.Observability.Settings;

namespace Sample.Observability
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenCoreTelemetry(this IServiceCollection services, OpenTelemetrySettings settings)
        {
            services.AddSingleton<ICoreTelemetry, OpenTelemetryAdapter>();

            return services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetSampler(new AlwaysOnSampler())
                    .AddSource(settings.ServiceName, nameof(OpenTelemetryAdapter))
                    .AddSource(settings.Sources.ToArray())
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: settings.ServiceName, serviceVersion: settings.ServiceVersion))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(jaegerOptions =>
                        {
                            jaegerOptions.AgentHost = settings.JaegerExporterHost; // Use name from docker-compose file, not "localhost";
                            jaegerOptions.AgentPort = settings.JaegerExporterPort;
                        });
            });
        }
    }
}