using static CongEspVilaGuilhermeApi.Domain.Entities.IDomainValidations;

namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class TerritoryCard: IDomainValidations
    {
        public int CardId { get; set; }
        public string Neighborhood { get; set; }
        public List<Direction> Directions { get; set; }

        public DomainError[] CheckErrors()
        {
            var errors = new List<DomainError>();
            DomainError.Null(nameof(Neighborhood), Neighborhood, errors);
            Directions.ForEach((d) => {
                var fieldPrefix = $"{nameof(Directions)}[{Directions.IndexOf(d)}].";
                DomainError.Null($"{fieldPrefix}{nameof(d.StreetName)}", d.StreetName, errors);
                DomainError.Null($"{fieldPrefix}{nameof(d.HouseNumber)}", d.HouseNumber, errors);
            });
            return errors.ToArray();
        }
    }
}
