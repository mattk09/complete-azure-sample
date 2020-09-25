using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Prometheus;
using Sample.Extensions;
using Sample.Extensions.Configurations;
using Sample.Extensions.Interfaces;
using Sample.Observability;
using Sample.Observability.Weather;
using Sample.Services;
using Sample.Services.Weather;
using Sample.Settings;
using Sample.Telemetry;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Read 'SampleSettings'
            var settings = new SampleSettings();
            this.Configuration.Bind(settings);

            services.AddSingleton<IWeatherForecaster, WeatherForecaster>();
            services.AddSingleton<IWeatherForecasterObservability, WeatherForecasterObservability>();

            services.AddSingleton<ISampleObservability, SampleObservability>();

            // Add the proper ISampleStorage and related services based on configuration
            services.AddSampleStorage(settings);

            services.AddControllers();

            // Add auth if configured
            services.AddAzureAdAuthentication(settings.Authentication);

            // Register the Swagger services
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Sample API";
                if (settings.Authentication.Enabled)
                {
                    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Paste at the textbox bellow: Bearer {your JWT id_token}.",
                    });

                    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                }
            });

            if (settings.Features.Telemetry == TelemetrySdk.OpenTelemetry)
            {
                services.AddTransient<ICoreTelemetry, OpenTelemetryAdapter>();
                services.AddOpenTelemetryTracing((builder) =>
                    builder
                        .AddSource("Samples.SampleClient", "Samples.SampleServer")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddJaegerExporter(jaegerOptions =>
                        {
                            jaegerOptions.ServiceName = "test-jaeger";
                            jaegerOptions.AgentHost = "localhost";
                            jaegerOptions.AgentPort = 6831;
                        })
                        .AddConsoleExporter());
            }
            else if (settings.Features.Telemetry == TelemetrySdk.ApplicationInsights)
            {
                // Use Application Insights
                // By default this will look for 'ApplicationInsights:InstrumentationKey' in the configuration.
                // This is added automatically by our 'AddAzureKeyVault' call in Program.cs
                services.AddApplicationInsightsTelemetry(this.Configuration);
                services.AddSingleton<ICoreTelemetry, ApplicationInsightsAdapter>();
            }
            else
            {
                // Telemetry set to None
                services.AddSingleton<ICoreTelemetry, NullCoreTelemetry>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Use middleware to route / to swagger
            app.Use(async (context, nextAsync) =>
            {
                if (context.Request.Path.Value == "/")
                {
                    // Rewrite and continue processing
                    context.Request.Path = "/swagger";
                }

                await nextAsync();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }

        // Move these closer to implementation
        private class WeatherForecasterObservability : IWeatherForecasterObservability
        {
            private readonly ICoreTelemetry telemetry;

            public WeatherForecasterObservability(ICoreTelemetry telemetry)
            {
                this.telemetry = telemetry;
            }

            public IDisposable GetForecast()
            {
                var span = this.telemetry.StartSpanActivity(nameof(GetForecast));
                span.SetAttribute("TestKey", "TestValue");

                return span;
            }
        }

        private class SampleObservability : ISampleObservability
        {
            private readonly ICoreTelemetry telemetry;

            public SampleObservability(ICoreTelemetry telemetry)
            {
                this.telemetry = telemetry;
            }

            // Consider a typed payload instead of primitives
            public IDisposable StartOperation(int depth, int sequence)
            {
                var span = this.telemetry.StartSpanActivity(nameof(StartOperation));
                span.SetAttribute("Depth", depth);
                span.SetAttribute("Sequence", sequence);

                return span;
            }

            public IDisposable StartSubOperation(int sequenceId)
            {
                var span = this.telemetry.StartSpanActivity(nameof(StartSubOperation));
                span.SetAttribute("SequenceId", sequenceId);

                return span;
            }
        }
    }
}
