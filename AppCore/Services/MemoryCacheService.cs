using CongEspVilaGuilhermeApi.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CongEspVilaGuilhermeApi.AppCore.Services;

public class MemoryCacheService: ICacheService
{
    private readonly IMemoryCache cache;
    private readonly ILoggerService logger;

    public MemoryCacheService(IMemoryCache cache, ILoggerService logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public Task Clear(string key)
    {
        cache.Remove(key);
        logger.Log($"[MemoryCache][{key}] Clear");
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, Func<ICacheEntry, Task<T>> factory) where T : class
    {
        logger.Log($"[MemoryCache][{key}] GetAsync");
        return cache.GetOrCreateAsync(key, factory);    
    }

    public Task SetAsync<T>(string key, T value) where T : class
    {
        cache.Set(key, value);
        logger.Log($"[MemoryCache][{key}] SetAsync");
        return Task.CompletedTask;
    }
}