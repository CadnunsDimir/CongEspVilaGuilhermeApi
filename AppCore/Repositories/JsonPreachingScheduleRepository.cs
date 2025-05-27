using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    class JsonPreachingScheduleRepository : IPreachingScheduleRepository
    {
        private readonly LoadFileService loadFileService;

        public JsonPreachingScheduleRepository(LoadFileService loadFileService)
        {
            this.loadFileService = loadFileService;
        }
        public List<FixedPreachingDay> GetAllFixedPreachingDays()
        {
            return loadFileService.LoadFileAsJson<List<FixedPreachingDay>>("PreachingSchedule.json") ?? new List<FixedPreachingDay>();
        }

        public void RegisterFixedPreachingDay(FixedPreachingDay fixedPreachingDay)
        {
            var fixedPreachingDays = GetAllFixedPreachingDays();
            fixedPreachingDays.RemoveAll(x => x.Id == fixedPreachingDay.Id);

            if (fixedPreachingDay.Id == null)
            {             
                fixedPreachingDay.Id = Guid.NewGuid().ToString();
            }

            fixedPreachingDays.Add(fixedPreachingDay);
            fixedPreachingDays = fixedPreachingDays.OrderBy(x => x.DayOfWeek).ThenBy(x => x.Hour).ToList();

            loadFileService.SaveFileAsJson("PreachingSchedule.json", fixedPreachingDays);
        }

        public void RegisterSpecialPreachingDay(SpecialPreachingDay fixedDay)
        {
            throw new NotImplementedException();
        }
    }
}