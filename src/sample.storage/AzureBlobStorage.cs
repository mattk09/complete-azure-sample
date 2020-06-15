using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Options;

using Sample.Core.Storage;
using Sample.Storage.Settings;

namespace Sample.Storage
{
    // This is a simple implementation to write to azure storage.  It lacks
    // a lot of validation and has some race conditions.  It is only  meant to
    // show a simple example of wiring up your app to real storage access.
    // DO NOT USE for production services.
    public class AzureBlobStorage : ISampleStorage
    {
        private readonly AzureStorageSettings settings;
        private readonly BlobContainerClient blobContainerClient;

        public AzureBlobStorage(IOptions<AzureStorageSettings> settings)
        {
            this.settings = settings?.Value;

            this.blobContainerClient = new BlobContainerClient(this.settings.ConnectionString, this.settings.ContainerName);
        }

        public async IAsyncEnumerable<string> GetKeysAsync()
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();

            await foreach (var blob in this.blobContainerClient.GetBlobsAsync())
            {
                yield return blob.Name;
            }
        }

        public async Task CreateAsync(string key, Stream value)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();

            await this.blobContainerClient.DeleteBlobIfExistsAsync(key);

            await this.blobContainerClient.UploadBlobAsync(key, value);
        }

        public async Task<Stream> GetAsync(string key)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();

            var client = this.blobContainerClient.GetBlobClient(key);

            if (!(await client.ExistsAsync()))
            {
                throw new KeyNotFoundException("key");
            }

            var blob = await client.DownloadAsync();

            return blob.Value.Content;
        }

        public async Task RemoveAsync(string key)
        {
            await this.blobContainerClient.CreateIfNotExistsAsync();

            var client = this.blobContainerClient.GetBlobClient(key);

            await client.DeleteIfExistsAsync();
        }
    }
}