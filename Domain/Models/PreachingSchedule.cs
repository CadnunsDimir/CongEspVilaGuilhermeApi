using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;

namespace CongEspVilaGuilhermeApi.Domain.Models
{
    public class PreachingSchedule
    {
        public required List<FixedPreachingDay> FixedPreachingDays { get; set; }
        public required List<SpecialPreachingDay> SpecialDays { get; set; }
    }
}