namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class TerritoryCard
    {
        public int CardId { get; set; }
        public string Neighborhood { get; set; }
        public List<Direction> Directions { get; set; }
    }
}
