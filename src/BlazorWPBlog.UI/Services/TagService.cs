using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WordPressPCL;

namespace BlazorWPBlog.UI.Services
{
    public interface ITagService
    {
        Task<WordPressPCL.Models.Tag> GetTagAsync(int id);
    }

    public class TagService : ITagService
    {
        private readonly WordPressClient _client;
        private readonly ILogger<TagService> _logger;
        private static readonly SemaphoreSlim _semaphore;
        private static IDictionary<int, WordPressPCL.Models.Tag> _tags;

        static TagService()
        {
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public TagService(WordPressClient client, ILogger<TagService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<WordPressPCL.Models.Tag> GetTagAsync(int id)
        {
            await EnsureTagsAsync(_client);

            _tags.TryGetValue(id, out var tag);
            return tag;
        }

        private async Task EnsureTagsAsync(WordPressClient client)
        {
            if (null != _tags)
                return;

            await _semaphore.WaitAsync();
            try
            {
                if (null == _tags)
                {
                    _logger.LogInformation($"fetching all tags...");
                    var tags = await client.Tags.GetAll();
                    _tags = tags?.ToDictionary(t => t.Id);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}