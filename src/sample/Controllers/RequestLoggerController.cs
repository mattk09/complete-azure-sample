using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.Core.Storage;

namespace Sample.Controllers
{
    [ApiController]
    [Route("requests")]
    public class RequestLoggerController : ControllerBase
    {
        private readonly ILogger<RequestLoggerController> logger;
        private readonly ISampleStorage storage;

        public RequestLoggerController(ILogger<RequestLoggerController> logger, ISampleStorage storage)
        {
            this.logger = logger;
            this.storage = storage;
        }

        [HttpGet("keys")]
        public IAsyncEnumerable<string> GetKeysAsync()
        {
            return this.storage.GetKeysAsync();
        }

        [HttpGet]
        public async Task<Stream> GetAsync([FromQuery] string key)
        {
            var stream = await this.storage.GetAsync(key);

            return stream;
        }

        [HttpPost("{basePath}/{*relativePathWithoutQuery}")]
        public async Task<IActionResult> PostAsync(string basePath, string relativePathWithoutQuery)
        {
            using var scope = this.logger.BeginScope(new Dictionary<string, object> { { "FullPath", $"{basePath}/{relativePathWithoutQuery}" } });

            var context = this.Request.HttpContext;

            // Log to storage
            await this.storage.CreateAsync(
                $"{basePath}/{relativePathWithoutQuery}",
                context.Request.Body);

            // Log to loggers
            this.logger.LogInformation($"Http Request Information: " +
                $"Method: {context.Request.Method} " +
                $"Schema: {context.Request.Scheme} " +
                $"Host: {context.Request.Host} " +
                $"Path: {context.Request.Path} " +
                $"QueryString: {context.Request.QueryString}");

            return Ok();
        }
    }
}
