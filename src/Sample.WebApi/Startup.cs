using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using Sample.Extensions;
using Sample.Services;
using Sample.Services.Weather;
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

            // Register the Swagger services
            services.AddOpenApiDocument();
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

            // Use middleware to route '/' to swagger
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
