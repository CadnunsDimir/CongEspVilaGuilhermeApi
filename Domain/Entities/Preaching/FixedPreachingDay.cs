namespace CongEspVilaGuilhermeApi.Domain.Entities.Preaching
{
    public class FixedPreachingDay : PreachingDay
    {
        public string? Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}