using CongEspVilaGuilhermeApi.AppCore.Models;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;

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
        Task<List<TerritoryCard>> GetAll();
        Task UpdateMany(List<TerritoryCard> territories);
        Task UpdateShareableIdAsync(int cardId, Guid id);
        Task<TerritoryCard?> GetByShareId(Guid cardId);
        Task<List<TerritoryMapMarkers>> GetFullMapMarkers();
        Task<int> CountAllDirections();
    }
}
