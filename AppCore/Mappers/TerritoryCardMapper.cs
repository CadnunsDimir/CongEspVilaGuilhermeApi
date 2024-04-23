using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using Newtonsoft.Json;

namespace CongEspVilaGuilhermeApi.Services.Mappers
{
    public class TerritoryCardMapper : IDynamoDbEntityMapper<TerritoryCard>
    {
        public class Keys
        {
            public static readonly string CardId = "card_id";
            internal static readonly string Neighborhood = "neighborhood";
            internal static readonly string Directions  = "directions";
        }
        public Document ToDynamoDocument(TerritoryCard card)    
        {
            var document = new Document();
            document[Keys.CardId] = card.CardId;
            document[Keys.Neighborhood] = card.Neighborhood;
            document[Keys.Directions] = JsonConvert.SerializeObject(card.Directions);
            return document;
        }

        public TerritoryCard ToEntity(Document document) => new TerritoryCard
        {
            CardId = Convert.ToInt32(document[Keys.CardId]),
            Neighborhood = document[Keys.Neighborhood],
            Directions = JsonConvert.DeserializeObject<List<Directions>>(document[Keys.Directions].AsString()) ?? new List<Directions>()
        };
    }
}