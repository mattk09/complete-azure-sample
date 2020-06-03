using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sample.Core.Storage
{
    public interface ISampleStorage
    {
        IAsyncEnumerable<string> GetKeysAsync();

        Task CreateAsync(string key, Stream value);

        Task<Stream> GetAsync(string key);

        Task RemoveAsync(string key);
    }
}
