using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Exceptions;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.Services;
using System.Linq;
using System.Threading.Tasks;

namespace CongEspVilaGuilhermeApi.Domain.UseCases;

public class TerritoryUseCases
{
    private readonly ITerritoryRepository repository;
    private readonly ICacheService cache;
    private readonly ILoggerService logger;
    private readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public TerritoryUseCases(ITerritoryRepository repository, ICacheService cache, ILoggerService logger)
    {
        this.repository = repository;
        this.cache = cache;
        this.logger = logger;
    }

    public async Task<List<int>> GetCardsAsync()
    {
        var data = await cache.GetAsync(nameof(repository.GetCardsAsync), async config =>
        {
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetCardsAsync();
            logger.Log("loading from db");
            return data ?? new List<int>();
        });

        return data!;
    }

    public async Task Create(TerritoryCard card)
    {
        var errors = card.CheckErrors();
        if (errors.Length > 0)
            throw new DomainEntityException(errors);
        await ClearCacheMap();
        await repository.Create(card);
    }

    public async Task Update(TerritoryCard card)
    {
        await repository.Update(card);
        await cache.SetAsync(TerritoryCardCacheKey(card.CardId), card);
        await ClearCacheMap();
    }

    private static string TerritoryCardCacheKey(int cardId) => typeof(TerritoryCard).Name + "_" + cardId;

    public async Task<TerritoryCard?> GetCardAsync(int id)
    {
        var data = await cache.GetAsync(TerritoryCardCacheKey(id), async config =>
        {
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetCardAsync(id);
            logger.Log("loading from db");
            if (data == null)
                throw new EntityNotFoundException<TerritoryCard>();
            return data;
        });

        return data;
    }

    public async Task UpdateDirection(int cardId, Direction direction)
    {
        await repository.UpdateDirection(cardId, direction);
        await cache.Clear(TerritoryCardCacheKey(cardId));
        await ClearCacheMap();
    }

    public async Task Delete(int id)
    {
        await repository.Delete(id);
        await cache.Clear(TerritoryCardCacheKey(id));
        await ClearCacheMap();
    }

    internal async Task<Guid> GetShareableId(int cardId)
    {
        var id = Guid.NewGuid();
        await repository.UpdateShareableIdAsync(cardId, id);
        return id;
    }

    internal Task<TerritoryCard?> GetCardByShareId(Guid cardId)
    {
        return repository.GetByShareId(cardId);
    }

    public async Task<FullMap> GetFullMap()
    {
        var data = await cache.GetAsync(nameof(repository.GetFullMapMarkers), async config =>
        {
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetFullMapMarkers();
            return data;
        });

        var count = await cache.GetAsync(nameof(repository.CountAllDirections), async config =>
        {
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.CountAllDirections();
            return data.ToString();
        });

        var cardsToCheck = await cache.GetAsync(nameof(repository.GetCardsToFixCoordinates), async config =>
        {
            config.SlidingExpiration = CacheExpiration;
            var data = await repository.GetCardsToFixCoordinates();
            return data;
        });

        return new FullMap
        {
            MapMarkers = data!,
            TotalAdresses = Convert.ToInt32(count),
            CheckCoordinatesOnCards = cardsToCheck!
        };
    }

    private async Task ClearCacheMap()
    {
        await cache.Clear(nameof(repository.GetCardsAsync));
        await cache.Clear(nameof(repository.GetFullMapMarkers));
        await cache.Clear(nameof(repository.GetCardsToFixCoordinates));
        await cache.Clear(nameof(repository.CountAllDirections));
        await cache.Clear(nameof(getCardsCoodinatesAsync));
    }

    public async Task MoveDirections(DirectionsExchange move)
    {
        var originCard = await GetCardAsync(move.OriginCardId);
        var destinationCard = await GetCardAsync(move.DestinationCardId);

        CheckCardsNotNull(originCard, destinationCard);

        var directionMoved = ExecuteDirectionMovement(originCard!, destinationCard!, move);

        if (directionMoved)
        {
            await repository.UpdateMany(destinationCard!, originCard!);
            await ClearCacheCards(move);
        }
    }

    private async Task ClearCacheCards(DirectionsExchange move)
    {
        await cache.Clear(TerritoryCardCacheKey(move.OriginCardId));
        await cache.Clear(TerritoryCardCacheKey(move.DestinationCardId));
    }

    private static bool ExecuteDirectionMovement(TerritoryCard originCard, TerritoryCard destinationCard, DirectionsExchange move)
    {
        var directionMoved = false;
        var filtered = from direction in move.Directions
                       where originCard.HasDirection(direction)
                       select direction;

        foreach (var direction in filtered)
        {
            originCard.MoveTo(direction, destinationCard);
            directionMoved = true;
        }

        return directionMoved;
    }

    private static void CheckCardsNotNull(TerritoryCard? originCard, TerritoryCard? destinationCard)
    {
        if (originCard == null || destinationCard == null)
            throw new EntityNotFoundException<TerritoryCard>();
    }

    public async Task<List<TerritoryCenterCoordinates>> getCardsCoodinatesAsync()
    {

        var data = await cache.GetAsync(nameof(getCardsCoodinatesAsync), async config =>
        {
            config.SlidingExpiration = CacheExpiration;

            var cards = await this.repository.GetAll();
            logger.Log("loading from db");
            return cards.Select(c =>
                {
                    var cardLat = c.Directions.Average(x => x.Lat) ?? 0;
                    var cardLong = c.Directions.Average(x => x.Long) ?? 0;
                    return new TerritoryCenterCoordinates(c.CardId, cardLat, cardLong);
                })
                .Where(x => Math.Abs(x.Lat) > 1e-6 && Math.Abs(x.Long) > 1e-6)
                .ToList();
        });

        return data!;

        
    }
}
