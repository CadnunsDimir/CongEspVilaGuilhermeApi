using CongEspVilaGuilhermeApi.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace CongEspVilaGuilhermeApi.Domain.Services
{
    public interface ICacheService
    {
        Task Clear(string v);
        Task<T?> GetAsync<T>(string key, Func<ICacheEntry, Task<T>> factory) where T : class;
        Task SetAsync<T>(string key, T value) where T : class;
    }
}