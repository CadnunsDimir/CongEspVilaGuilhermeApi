using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TerritoryController : ControllerBase
    {
        private readonly ITerritoryRepository repository;
        private readonly TerritoryUseCases useCases;

        public TerritoryController(ITerritoryRepository repository, TerritoryUseCases useCases)
        {
            this.repository = repository;
            this.useCases = useCases;
        }

        [HttpGet]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task<List<int>> Index() => useCases.GetCardsAsync();

        [HttpGet("{id}")]
        public Task<TerritoryCard?> Details(int id) => useCases.GetCardAsync(id);

        [HttpPost]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task Create(TerritoryCard card) => useCases.Create(card); 

        [HttpPut]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task Edit(TerritoryCard card) => useCases.Update(card);

        [HttpPut("{cardId}/direction")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task EditDirection(int cardId, Direction direction) => 
            useCases.UpdateDirection(cardId, direction);

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task Delete(int id) => useCases.Delete(id);
    }
}
