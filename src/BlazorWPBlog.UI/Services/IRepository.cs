using System.Threading.Tasks;

namespace BlazorWPBlog.UI.Services
{
    public interface IRepository<TKey, TValue>
    {
        Task<TValue> GetByIdAsync(TKey id);
    }
}