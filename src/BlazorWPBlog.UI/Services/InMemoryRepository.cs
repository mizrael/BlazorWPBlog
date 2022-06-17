using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWPBlog.UI.Services
{
    public abstract class InMemoryRepository<TKey, TValue> : IRepository<TKey, TValue>
    {
        private readonly SemaphoreSlim _semaphore;
        private IReadOnlyDictionary<TKey, TValue> _items;
        private readonly Func<Task<IReadOnlyDictionary<TKey, TValue>>> _factory;

        public InMemoryRepository(Func<Task<IReadOnlyDictionary<TKey, TValue>>> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<TValue> GetByIdAsync(TKey id)
        {
            await EnsureItemsAsync();
            _items.TryGetValue(id, out var item);
            return item;
        }

        private async Task EnsureItemsAsync()
        {
            if (null != _items)
                return;

            await _semaphore.WaitAsync();
            try
            {
                _items ??= await _factory();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}