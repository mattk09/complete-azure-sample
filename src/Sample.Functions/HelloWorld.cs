using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sample.Observability;

namespace Sample.Functions
{
    public class HelloWorld
    {
        private readonly IConfiguration configuration;
        private readonly ICoreTelemetry telemetry;

        public HelloWorld(IConfiguration configuration, ICoreTelemetry telemetry)
        {
            this.configuration = configuration;
            this.telemetry = telemetry;
        }

        [FunctionName("HelloWorld")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            using var span = this.telemetry.Start($"{nameof(HelloWorld)}-{nameof(RunAsync)}");

            log.LogInformation($"C# HTTP trigger function processed a request: {this.telemetry.GetType().FullName}");

            using var reader = new StreamReader(request.Body);
            var requestBody = await reader.ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = request.Query["name"].ToString() ?? data?.name;

            var responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
