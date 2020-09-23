using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Models;
using Sample.Observability;
using Sample.Services;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleSpanProducerController : ControllerBase
    {
        // TODO: Create a provider
        private static readonly HttpClient client = new HttpClient();

        private readonly ISampleObservability observability;

        public SampleSpanProducerController(ISampleObservability observability)
        {
            this.observability = observability;
        }

        [HttpGet("start")]
        public async Task GetAsync([FromQuery] int depth = 1, [FromQuery] int sequences = 2, [FromQuery] double chanceOfCalling = 0.5)
        {
            Random random = new Random();
            using var operation = this.observability.StartOperation(depth, sequences);

            await Task.Delay(TimeSpan.FromSeconds(random.Next(2)));

            if (depth > 0)
            {
                for (int i = 0; i < sequences; ++i)
                {
                    if (random.NextDouble() < chanceOfCalling)
                    {
                        using var suboperation = this.observability.StartOperation(depth, sequences);

                        await client.GetAsync(new Uri($"http://localhost:5000/SampleSpanProducer/next?depth={depth - 1}&sequences={sequences}&chanceOfCalling={chanceOfCalling}"));
                    }
                }
            }

            // await Task.Delay(TimeSpan.FromSeconds(random.Next(2)));
        }

        [HttpGet("next")]
        public async Task GetNextAsync([FromQuery] int depth = 1, [FromQuery] int sequences = 2, [FromQuery] double chanceOfCalling = 0.5)
        {
            Random random = new Random();
            using var operation = this.observability.StartOperation(depth, sequences);

            // Insert delay
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (depth > 0)
            {
                for (int i = 0; i < sequences; ++i)
                {
                    if (random.NextDouble() < chanceOfCalling)
                    {
                        using var suboperation = this.observability.StartSubOperation(i);

                        // await client.GetAsync(new Uri($"http://localhost:5000/SampleSpanProducer/next?depth={depth - 1}&sequences={sequences}&chanceOfCalling={chanceOfCalling}"));
                        await this.GetNextAsync(depth - 1, sequences, chanceOfCalling);
                    }
                }
            }

            // await Task.Delay(TimeSpan.FromSeconds(random.Next(2)));
        }
    }
}
