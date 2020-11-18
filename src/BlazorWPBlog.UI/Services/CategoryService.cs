using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WordPressPCL;

namespace BlazorWPBlog.UI.Services
{
    public interface ICategoryService
    {
        Task<WordPressPCL.Models.Category> GetTagAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly WordPressClient _client;
        private readonly ILogger<CategoryService> _logger;
        private static readonly SemaphoreSlim _semaphore;
        private static IDictionary<int, WordPressPCL.Models.Category> _categories;

        static CategoryService()
        {
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public CategoryService(WordPressClient client, ILogger<CategoryService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<WordPressPCL.Models.Category> GetTagAsync(int id)
        {
            await EnsureTagsAsync(_client);

            _categories.TryGetValue(id, out var category);
            return category;
        }

        private async Task EnsureTagsAsync(WordPressClient client)
        {
            if (null != _categories)
                return;

            await _semaphore.WaitAsync();
            try
            {
                if (null == _categories)
                {
                    _logger.LogInformation($"fetching all categories...");
                    var categories = await client.Categories.GetAll();
                    _categories = categories?.ToDictionary(t => t.Id);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}