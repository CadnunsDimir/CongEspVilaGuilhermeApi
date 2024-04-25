namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class Direction
    {
        public required string StreetName { get; set; }
        public required string HouseNumber { get; set; }
        public string? ComplementaryInfo { get; set; }
        public float? Lat { get; set; }
        public float? Long { get; set; }
    }
}