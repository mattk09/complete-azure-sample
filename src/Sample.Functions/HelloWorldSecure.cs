using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sample.Functions
{
    public class HelloWorldSecure
    {
        private readonly IConfiguration configuration;

        public HelloWorldSecure(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [FunctionName("HelloWorldSecure")]
        public async Task<IActionResult> RunSecureAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a secure request.");

            await Task.CompletedTask;

            return new OkObjectResult("Secured");
        }
    }
}
