using System;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using Sample.Core;
using Sample.Core.Storage;
using Sample.Extensions;
using Sample.Extensions.Configurations;
using Sample.Extensions.Interfaces;
using Sample.Settings;

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

            // By default this will look for 'ApplicationInsights:InstrumentationKey' in the configuration.
            // This is added automatically by our 'AddAzureKeyVault' call in Program.cs
            services.AddApplicationInsightsTelemetry(this.Configuration);

            services.AddSingleton<IWeatherForecaster, WeatherForecaster>();

            // Add the the proper ISampleStorage and related services based on configuration
            services.AddSampleStorage(settings);

            services.AddControllers();

            var authConfig = Configuration.GetSection("Authentication")
                .Get<AuthenticationConfiguration>();

            services.AddSingleton<IAuthenticationConfiguration>(authConfig);
            services.AddAzureAdAuthentication(authConfig);

            // Register the Swagger services
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Sample API";
                if (authConfig.Enabled)
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
                endpoints.MapControllers();
            });
        }
    }
}
