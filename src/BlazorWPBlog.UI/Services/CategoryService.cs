using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;

namespace BlazorWPBlog.UI.Services
{
    public interface ICategoryService : IRepository<int, Category>
    {
    }

    public class CategoryService : InMemoryRepository<int, Category>, ICategoryService
    {
        private readonly WordPressClient _client;
        private readonly ILogger<CategoryService> _logger;
  
        public CategoryService(WordPressClient client, ILogger<CategoryService> logger)
             : base(() => EnsureCategories(client, logger))
        {
            _client = client;
            _logger = logger;
        }

        private async static Task<IReadOnlyDictionary<int, Category>> EnsureCategories(WordPressClient client, ILogger logger)
        {
            logger.LogInformation($"fetching all categories...");
            var items = await client.Categories.GetAllAsync();
            return items?.ToDictionary(t => t.Id);
        }
    }
}