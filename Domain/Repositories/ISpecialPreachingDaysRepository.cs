using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface ISpecialPreachingDaysRepository
    {
        void createOrUpdate(SpecialPreachingDay specialPreachingDay);
        SpecialPreachingDay? GetSpecialDayByDate(DateTime today);
        List<SpecialPreachingDay> GetSpecialDaysByMonth(int month, int year);
    }
}