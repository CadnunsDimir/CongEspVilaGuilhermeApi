using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TerritoryController : ControllerBase
    {
        private readonly ITerritoryRepository repository;

        public TerritoryController(ITerritoryRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public Task<List<int>> Index() => repository.GetCardsAsync();

        [HttpGet("{id}")]
        public Task<TerritoryCard?> Details(int id) => repository.GetCardAsync(id);

        [HttpPost]
        public Task Create(TerritoryCard card)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public Task Edit(TerritoryCard card)
        {
            return repository.Update(card);
        }

        [HttpPut("{cardId}/direction")]
        public Task EditDirection(int cardId, Direction direction)
        {
            return repository.UpdateDirection(cardId, direction);
        }

        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return repository.Delete(id);
        }
    }
}
