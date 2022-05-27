using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sample.Observability;

namespace Sample.Functions
{
    public class HelloWorldSecure
    {
        private readonly IConfiguration configuration;
        private readonly ICoreTelemetry telemetry;

        public HelloWorldSecure(IConfiguration configuration, ICoreTelemetry telemetry)
        {
            this.configuration = configuration;
            this.telemetry = telemetry;
        }

        [FunctionName("HelloWorldSecure")]
        public async Task<IActionResult> RunSecureAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            using var span = this.telemetry.Start($"{nameof(HelloWorldSecure)}-{nameof(RunSecureAsync)}");

            log.LogInformation("C# HTTP trigger function processed a secure request.");

            await Task.CompletedTask;

            return new OkObjectResult("Secured");
        }
    }
}
