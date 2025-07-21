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

            var updatedSpecialDays = specialPreachingDaysRepository.GetSpecialDaysByMonth(monthYear.Month, monthYear.Year);

            var specialDays = currentMonthHolidays.Select(d =>
            {
                var date = new DateTime(monthYear.Year, monthYear.Month, d.dayOfMonth, 0, 0, 0, DateTimeKind.Local);
                var savedSpecialDay = updatedSpecialDays.FirstOrDefault(x=> x.Date.Day == date.Day);
                if (savedSpecialDay != null)
                {
                    updatedSpecialDays.Remove(savedSpecialDay);
                    return savedSpecialDay;
                }

                return new SpecialPreachingDay
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

            specialDays.AddRange(updatedSpecialDays);

            return new PreachingSchedule
            {
                FixedPreachingDays = repository.GetAllFixedPreachingDays(),
                SpecialDays = specialDays.OrderBy(x=>x.Date).ToList()
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