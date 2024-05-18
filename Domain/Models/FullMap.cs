
namespace CongEspVilaGuilhermeApi.Domain.Models;

public class FullMap
{
    public List<TerritoryMapMarkers> MapMarkers { get; set; }
    public int TotalAdresses { get; set; }
    public List<int> CheckCoordinatesOnCards { get; set; }
}