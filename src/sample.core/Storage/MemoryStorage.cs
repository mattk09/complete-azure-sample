using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Sample.Core.Storage
{
    public class MemoryStorage : ISampleStorage
    {
        private readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();
        private readonly ConcurrentDictionary<string, byte[]> storage = new ConcurrentDictionary<string, byte[]>();

        public async IAsyncEnumerable<string> GetKeysAsync()
        {
            foreach (var key in this.storage.Keys)
            {
                yield return key;
            }

            await Task.CompletedTask;
        }

        public async Task CreateAsync(string key, Stream value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using var copyStream = new MemoryStream();

            await value.CopyToAsync(copyStream);

            var byteBuffer = copyStream.ToArray();

            // Each create will make a new buffer
            this.storage.AddOrUpdate(key, byteBuffer, (k, v) => byteBuffer);
        }

        public Task<Stream> GetAsync(string key)
        {
            if (this.storage.TryGetValue(key, out var byteBuffer))
            {
                // This will copy from the buffer to recycled buffers
                var copyStream = this.manager.GetStream(byteBuffer);

                return Task.FromResult<Stream>(copyStream);
            }

            throw new KeyNotFoundException(key);
        }

        public Task RemoveAsync(string key)
        {
            this.storage.TryRemove(key, out _);

            return Task.CompletedTask;
        }
    }
}
