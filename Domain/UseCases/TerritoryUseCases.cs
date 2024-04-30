

using System.Runtime.Serialization;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Exceptions;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.Services;

namespace CongEspVilaGuilhermeApi.Domain.UseCases;
public class TerritoryUseCases
{
    private readonly ITerritoryRepository repository;
    private readonly ICacheService cache;

    private readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public TerritoryUseCases(ITerritoryRepository repository, ICacheService cache)
    {
        this.repository = repository;
        this.cache = cache;
    }

    public async Task<List<int>> GetCardsAsync()
    {
        var data = await cache.GetAsync(nameof(GetCardsAsync), async config =>{
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetCardsAsync();
            Console.WriteLine("loading from db");
            return data ?? new List<int>();
        });

        return data!;
    }

    public Task Create(TerritoryCard card){
        var errors = card.CheckErrors();
        if (errors.Length == 0)
        {
            return repository.Create(card);
        }
        throw new DomainEntityException(errors);
    }

    public async Task Update(TerritoryCard card)
    {
        await repository.Update(card);
        await cache.SetAsync(TerritoryCardCacheKey(card.CardId), card);
    }

    private string TerritoryCardCacheKey(int cardId) => typeof(TerritoryCard).Name + "_" + cardId;

    public async Task<TerritoryCard?> GetCardAsync(int id)
    {
        var data = await cache.GetAsync(TerritoryCardCacheKey(id), async config =>{
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetCardAsync(id);
            Console.WriteLine("loading from db");
            if(data == null)
                throw new Exception("not found");
            return data;
        });

        return data;
    }

    public async Task UpdateDirection(int cardId, Direction direction)
    {
        await repository.UpdateDirection(cardId, direction);
        await cache.Clear(TerritoryCardCacheKey(cardId));
    }

    public async Task Delete(int id)
    {
        await repository.Delete(id);
        await cache.Clear(TerritoryCardCacheKey(id));
    }
}