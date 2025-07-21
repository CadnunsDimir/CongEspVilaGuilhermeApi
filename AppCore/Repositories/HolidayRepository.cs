using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class HolidayRepository : IHolidaysRepository
    {
        private readonly List<Holiday> holidays = new List<Holiday>
        {
            new Holiday { Month = 7, dayOfMonth = 9 },
            new Holiday { Month = 9, dayOfMonth = 7 },
            new Holiday { Month = 10, dayOfMonth = 12 },
            new Holiday { Month = 11, dayOfMonth = 2 },
            new Holiday { Month = 11, dayOfMonth = 15 },
            new Holiday { Month = 11, dayOfMonth = 20 },
            new Holiday { Month = 12, dayOfMonth = 25 },
            new Holiday { Month = 12, dayOfMonth = 31 },
            new Holiday { Month = 1, dayOfMonth = 1 },
            new Holiday { Month = 1, dayOfMonth = 25 }
        };

        private readonly Dictionary<int, List<Holiday>> notFixedHolidays = new Dictionary<int, List<Holiday>>{
            { 2026, new List<Holiday> {
                new Holiday { Month = 2, dayOfMonth = 16 },
                new Holiday { Month = 2, dayOfMonth = 17 },
                new Holiday { Month = 4, dayOfMonth = 3 },
                new Holiday { Month = 4, dayOfMonth = 21 }
            }}
            // TODO: definir feriados de 2027
            // 2027 = [
            //     //definir datas
            // ]
        };

        public List<Holiday> GetHolidays(int month, int year)
        {
            var monthHolidays = this.holidays.Where(x => x.Month == month).ToList();
            if (notFixedHolidays.ContainsKey(year))
            {
                monthHolidays.AddRange(notFixedHolidays[year].Where(x => x.Month == month));
            }
            return monthHolidays;
        }
    }
}