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
        [ValidateAntiForgeryToken]
        public dynamic Create(TerritoryCard card)
        {
            return new { };
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public dynamic Edit(int id, TerritoryCard card)
        {
            return new { };
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public dynamic Delete(int id)
        {
            return new { };
        }
    }
}
