using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Models
{
    public record DirectionsExchange(int OriginCardId, int DestinationCardId, List<Direction> Directions);
}
