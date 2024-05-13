using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;
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
            internal static readonly string IsDeleted = "is_deleted";
            internal static readonly string ShareId = "share_id";
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
            CardId = MapCardId(document),
            Neighborhood = document[Keys.Neighborhood],
            Directions = MapDirections(document)
        };

        public int MapCardId(Document document) => Convert.ToInt32(document[Keys.CardId]);
        public List<Direction> MapDirections(Document document) => JsonConvert.DeserializeObject<List<Direction>>(document[Keys.Directions].AsString()) ?? new List<Direction>();

        public List<TerritoryMapMarkers> ToMapMarkers(List<Document> documentList)
        {
            return documentList.SelectMany(doc =>
            {
                var cardId = MapCardId(doc);
                return MapDirections(doc)
                    
                    .Select((d,i)=> new { index = i + 1, d.Lat, d.Long })
                    .Where(x=> x.Lat !=null && x.Long != null)
                    .Select(d=> new TerritoryMapMarkers
                    {
                        CardId = cardId,
                        Lat = (float)d.Lat!,
                        Long = (float)d.Long!,
                        OrderPosition = d.index
                    });
            }).ToList();
        }
    }
}