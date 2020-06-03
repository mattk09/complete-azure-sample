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
        private readonly ConcurrentDictionary<string, Memory<byte>> storage = new ConcurrentDictionary<string, Memory<byte>>();

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

            var byteBuffer = copyStream.GetBuffer();
            Memory<byte> memory = new Memory<byte>(byteBuffer, 0, (int)copyStream.Length);

            copyStream.Seek(0, SeekOrigin.Begin);

            // Each create will make a new buffer
            this.storage.AddOrUpdate(key, memory, (k, v) => memory);
        }

        public Task<Stream> GetAsync(string key)
        {
            if (this.storage.TryGetValue(key, out var byteBuffer))
            {
                // This will copy from the buffer to recycled buffers
                MemoryStream copyStream = this.manager.GetStream(byteBuffer);

                return Task.FromResult<Stream>(copyStream);
            }

            throw new InvalidOperationException($"{key} not found");
        }

        public Task RemoveAsync(string key)
        {
            this.storage.TryRemove(key, out var stream);

            return Task.CompletedTask;
        }
    }
}
