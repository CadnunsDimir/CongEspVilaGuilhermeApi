namespace CongEspVilaGuilhermeApi.AppCore.Models
{
    public class ShareTerritoryCard
    {
        public Guid TemporaryId { get; set; }
        public string ExampleUrl { get; set; } = "/api/territory/{temporaryId}/public";
    }
}
