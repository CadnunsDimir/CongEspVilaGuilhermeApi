using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PreachingScheduleController
    {
        private readonly IPreachingScheduleRepository repository;
        public PreachingScheduleController(IPreachingScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public PreachingSchedule Get()
        {
            return new PreachingSchedule
            {
                FixedPreachingDays = repository.GetAllFixedPreachingDays()
            };
        }

        [HttpPost("fixed-day")]
        public IResult Post(FixedPreachingDay fixedDay)
        {
            repository.RegisterFixedPreachingDay(fixedDay);
            return Results.Ok(fixedDay);
        }

        [HttpPost("special-day")]
        public IResult PostSpecialDay(SpecialPreachingDay fixedDay)
        {
            repository.RegisterSpecialPreachingDay(fixedDay);
            return Results.Ok(fixedDay);
        }
    }
}