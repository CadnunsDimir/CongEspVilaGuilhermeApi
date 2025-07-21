using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.UseCases;
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
        private readonly PreachingScheduleUseCases useCases;
        public PreachingScheduleController(
            PreachingScheduleUseCases useCases)
        {
            this.useCases = useCases;
        }

        [HttpGet]
        public PreachingSchedule Get(int? month, int? year)
        {
            
            var date = month.HasValue && year.HasValue ?
                new DateTime(year.Value, month.Value, 1, 0, 0, 0) :
                DateTime.Today;

            return useCases.GetSchedule(date);
        }

        [HttpPost("fixed-day")]
        public IResult Post(FixedPreachingDay fixedDay)
        {
            useCases.RegisterFixedPreachingDay(fixedDay);
            return Results.Ok(fixedDay);
        }

        [HttpPost("special-day")]
        public IResult PostSpecialDay(SpecialPreachingDay fixedDay)
        {
            useCases.RegisterSpecialPreachingDay(fixedDay);
            return Results.Ok(fixedDay);
        }
    }
}