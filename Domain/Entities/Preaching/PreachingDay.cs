namespace CongEspVilaGuilhermeApi.Domain.Entities.Preaching
{
    public abstract class PreachingDay
    {        
        public TimeOnly Hour { get; set; }
        public required string FieldOverseer { get; set; }
        public required Place Place { get; set; }
    }
}