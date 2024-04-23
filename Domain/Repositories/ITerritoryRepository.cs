using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface ITerritoryRepository
    {
        Task Create(TerritoryCard card);
        Task<List<int>> GetCardsAsync();
        Task<TerritoryCard?> GetCardAsync(int id);
    }
}
