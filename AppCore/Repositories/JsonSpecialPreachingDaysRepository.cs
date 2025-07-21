using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    class JsonSpecialPreachingDaysRepository : ISpecialPreachingDaysRepository
    {
        private readonly LoadFileService loadFileService;
        private readonly String sprecialDaysFile = "SprecialDaysSchedule.json";
        private readonly List<SpecialPreachingDay> specialPreachingDaysCache = new List<SpecialPreachingDay>();

        public JsonSpecialPreachingDaysRepository(LoadFileService loadFileService)
        {
            this.loadFileService = loadFileService;
        }

        public SpecialPreachingDay? GetSpecialDayByDate(DateTime today)
        {
            updateCache();
            return specialPreachingDaysCache.FirstOrDefault(x => x.Date == DateOnly.FromDateTime(today));
        }

        private void updateCache()
        {
            if (specialPreachingDaysCache.Count == 0)
            {
                specialPreachingDaysCache.AddRange(GetAllSpecialDays());
            }
        }

        public void createOrUpdate(SpecialPreachingDay specialPreachingDay)
        {
            var allItens = GetAllSpecialDays();
            allItens.RemoveAll(x => x.Date == specialPreachingDay.Date);
            allItens.Add(specialPreachingDay);
            allItens = allItens.OrderBy(x => x.Date).ToList();
            loadFileService.SaveFileAsJson(sprecialDaysFile, allItens);
        }

        private List<SpecialPreachingDay> GetAllSpecialDays()
        {
            return loadFileService.LoadFileAsJson<List<SpecialPreachingDay>>(sprecialDaysFile) ??
                new List<SpecialPreachingDay>();
        }

        public List<SpecialPreachingDay> GetSpecialDaysByMonth(int month, int year)
        {
            updateCache();
            return this.specialPreachingDaysCache
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .ToList();
        }
    }
}