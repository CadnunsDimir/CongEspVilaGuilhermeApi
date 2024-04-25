using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface ITerritoryRepository
    {
        Task Create(TerritoryCard card);
        Task<List<int>> GetCardsAsync();
        Task<TerritoryCard?> GetCardAsync(int id);
        Task Update(TerritoryCard card);
        Task Delete(int id);
        Task UpdateDirection(int cardId, Direction direction);
    }
}
