using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CongEspVilaGuilhermeApi.Domain.Models;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface IHolidaysRepository
    {
        List<Holiday> GetHolidays(int month, int year);
    }
}