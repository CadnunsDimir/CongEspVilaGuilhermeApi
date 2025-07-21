using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    class JsonPreachingScheduleRepository : IPreachingScheduleRepository
    {
        private readonly LoadFileService loadFileService;
        private readonly String preachingScheduleJsonFile = "PreachingSchedule.json";
        private readonly String specialDaysFile = "SpecialDaysSchedule.json";
        private readonly List<SpecialPreachingDay> specialPreachingDaysCache = new List<SpecialPreachingDay>();

        public JsonPreachingScheduleRepository(LoadFileService loadFileService)
        {
            this.loadFileService = loadFileService;
        }
        public List<FixedPreachingDay> GetAllFixedPreachingDays()
        {
            return loadFileService.LoadFileAsJson<List<FixedPreachingDay>>(preachingScheduleJsonFile) ?? new List<FixedPreachingDay>();
        }

        public SpecialPreachingDay? GetSpecialDayByDate(DateTime today)
        {
            if (specialPreachingDaysCache.Count == 0)
            {
                specialPreachingDaysCache.AddRange(GetAllSpecialDays());
            }

            return specialPreachingDaysCache.FirstOrDefault(x => x.Date == DateOnly.FromDateTime(today));
        }

        public void createOrUpdate(FixedPreachingDay fixedPreachingDay)
        {
            var fixedPreachingDays = GetAllFixedPreachingDays();
            fixedPreachingDays.RemoveAll(x => x.Id == fixedPreachingDay.Id);

            if (fixedPreachingDay.Id == null)
            {             
                fixedPreachingDay.Id = Guid.NewGuid().ToString();
            }

            fixedPreachingDays.Add(fixedPreachingDay);
            fixedPreachingDays = fixedPreachingDays.OrderBy(x => x.DayOfWeek).ThenBy(x => x.Hour).ToList();

            loadFileService.SaveFileAsJson(preachingScheduleJsonFile, fixedPreachingDays);
        }

        public void RegisterSpecialPreachingDay(SpecialPreachingDay specialPreachingDay)
        {
            var allItens = GetAllSpecialDays();
            allItens.RemoveAll(x => x.Date == specialPreachingDay.Date);
            allItens.Add(specialPreachingDay);
            allItens = allItens.OrderBy(x => x.Date).ToList();
            loadFileService.SaveFileAsJson(specialDaysFile, allItens);
        }

        private List<SpecialPreachingDay> GetAllSpecialDays()
        {
            return loadFileService.LoadFileAsJson<List<SpecialPreachingDay>>(specialDaysFile) ??
                new List<SpecialPreachingDay>();
        }
    }
}