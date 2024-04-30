using CongEspVilaGuilhermeApi.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CongEspVilaGuilhermeApi.AppCore.Services;

public class MemoryCacheService: ICacheService
{
    private IMemoryCache cache;    

    public MemoryCacheService(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public Task Clear(string key)
    {
        cache.Remove(key);
        Console.WriteLine($"[MemoryCache][{key}] Clear");
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, Func<ICacheEntry, Task<T>> factory) where T : class
    {
        Console.WriteLine($"[MemoryCache][{key}] GetAsync");
        return cache.GetOrCreateAsync(key, factory);    
    }

    public Task SetAsync<T>(string key, T value) where T : class
    {
        cache.Set(key, value);
        Console.WriteLine($"[MemoryCache][{key}] SetAsync");
        return Task.CompletedTask;
    }
}