using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;

namespace BlazorWPBlog.UI.Services
{
    public interface ITagService : IRepository<int, Tag>
    {
    }

    public class TagService : InMemoryRepository<int, Tag>, ITagService
    {
        private readonly WordPressClient _client;
        private readonly ILogger<TagService> _logger;      

        public TagService(WordPressClient client, ILogger<TagService> logger)
            : base(() => EnsureTagsAsync(client, logger))
        {
            _client = client;
            _logger = logger;
        }

        private async static Task<IReadOnlyDictionary<int, Tag>> EnsureTagsAsync(WordPressClient client, ILogger logger)
        {
            logger.LogInformation($"fetching all tags...");
            var tags = await client.Tags.GetAllAsync();
            return tags?.ToDictionary(t => t.Id);
        }
    }
}