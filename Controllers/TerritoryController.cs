using CongEspVilaGuilhermeApi.AppCore.Models;
using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;
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
        private readonly TerritoryUseCases useCases;
        private readonly TerritoryRepositoryValidationService repositoryValidator;

        public TerritoryController(
            TerritoryUseCases useCases,
            TerritoryRepositoryValidationService repositoryValidator)
        {
            this.useCases = useCases;
            this.repositoryValidator = repositoryValidator;
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

        [HttpPost("move")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task Move([FromBody] DirectionsExchange move) =>
            useCases.MoveDirections(move);

        [HttpGet("{cardId}/share")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public async Task<ShareTerritoryCard> Share(int cardId)
        {
            Guid id = await useCases.GetShareableId(cardId);
            return new ShareTerritoryCard
            {
                TemporaryId = id
            };
        }

        [HttpGet("{cardId}/public")]
        [AllowAnonymous]
        public Task<TerritoryCard?> Share(Guid cardId) => useCases.GetCardByShareId(cardId);

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public Task Delete(int id) => useCases.Delete(id);

        [HttpGet("sync_tsv")]
        [Authorize(Roles = nameof(RoleTypes.TerritoryServant))]
        public dynamic SyncTsv()
        {
            _ = repositoryValidator.UpdateDbUsingOnlineSheetAsync();
            return new
            {
                message = "Update started"
            };
        }

        [HttpGet("full_map")]
        public Task<FullMap> FullMap() => useCases.GetFullMap();

    }
}
