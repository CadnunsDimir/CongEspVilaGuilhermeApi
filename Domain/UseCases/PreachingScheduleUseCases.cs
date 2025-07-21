using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.Domain.UseCases
{
    public class PreachingScheduleUseCases
    {
        private readonly IPreachingScheduleRepository repository;
        private readonly IHolidaysRepository holidaysRepository;
        private readonly ISpecialPreachingDaysRepository specialPreachingDaysRepository;

        public PreachingScheduleUseCases(
            IPreachingScheduleRepository repository,
            IHolidaysRepository holidaysRepository,
            ISpecialPreachingDaysRepository specialPreachingDaysRepository)
        {
            this.repository = repository;
            this.holidaysRepository = holidaysRepository;
            this.specialPreachingDaysRepository = specialPreachingDaysRepository;
        }
        public PreachingSchedule GetSchedule(DateTime monthYear)
        {
            var currentMonthHolidays = holidaysRepository.GetHolidays(monthYear.Month, monthYear.Year);

            var specialDays = currentMonthHolidays.Select(d =>
            {
                var date = new DateTime(monthYear.Year, monthYear.Month, d.dayOfMonth, 0, 0, 0, DateTimeKind.Local);
                var savedSpecialDay = specialPreachingDaysRepository.GetSpecialDayByDate(date);
                return savedSpecialDay ?? new SpecialPreachingDay
                {
                    Date = DateOnly.FromDateTime(date),
                    FieldOverseer = "",
                    Place = new Place
                    {
                        Adress = "",
                        Name = ""
                    },
                    Hour = TimeOnly.Parse("09:00", System.Globalization.CultureInfo.InvariantCulture)
                };
            }).ToList();

            return new PreachingSchedule
            {
                FixedPreachingDays = repository.GetAllFixedPreachingDays(),
                SpecialDays = specialDays
            };
        }

        public void RegisterFixedPreachingDay(FixedPreachingDay fixedDay)
        {
            repository.createOrUpdate(fixedDay);
        }

        public void RegisterSpecialPreachingDay(SpecialPreachingDay fixedDay)
        {
            specialPreachingDaysRepository.createOrUpdate(fixedDay);
        }
    }
}