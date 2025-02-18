namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class LifeAndMinistryAsignment
    {
        public required string Title { get; set; }
        public int Minutes { get; set; }
        public required string BrotherName { get; set; }        
    }
}