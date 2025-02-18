using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LifeAndMinistryController : ControllerBase
    {
        private LifeAndMinistryProgramService service;

        public LifeAndMinistryController(LifeAndMinistryProgramService service)
        {
            this.service = service;
        }

        [HttpGet("{date}")]
        public Task<LifeAndMinistryWeek> GetProgramByDate(DateTime date)
        {
            return service.GetProgramByDate(date);
        }

        [HttpPut]
        [Authorize(Roles = nameof(RoleTypes.LifeAndMinistryAdmins))]
        public Task UpdateWeek(LifeAndMinistryWeek week)
        {
            return service.UpdateWeek(week);
        }
    }
}
