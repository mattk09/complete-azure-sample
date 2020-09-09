﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Sample.Storage;

namespace Sample.Controllers
{
    [ApiController]
    [Route("requests")]
    public class RequestLoggerController : ControllerBase
    {
        private readonly IStorage storage;

        public RequestLoggerController(IStorage storage)
        {
            this.storage = storage;
        }

        [HttpGet("keys")]
        public IAsyncEnumerable<string> GetIdentifiersAsync()
        {
            return this.storage.GetIdentifiersAsync();
        }

        [HttpGet("{**relativePathWithoutQuery}")]
        public async Task<ActionResult<Stream>> GetAsync(string relativePathWithoutQuery)
        {
            if (string.IsNullOrWhiteSpace(relativePathWithoutQuery))
            {
                return this.BadRequest();
            }

            try
            {
                return await this.storage.GetAsync(relativePathWithoutQuery);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound();
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync([FromQuery] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this.BadRequest();
            }

            await this.storage.RemoveAsync(key);

            return this.Ok();
        }

        [HttpPost("{**relativePathWithoutQuery}")]
        public async Task<IActionResult> PostAsync(string relativePathWithoutQuery)
        {
            if (string.IsNullOrWhiteSpace(relativePathWithoutQuery))
            {
                return this.BadRequest();
            }

            await this.storage.CreateAsync(relativePathWithoutQuery, this.HttpContext.Request.Body);

            return this.Ok();
        }
    }
}