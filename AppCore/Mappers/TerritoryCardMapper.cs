using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;
using Newtonsoft.Json;

namespace CongEspVilaGuilhermeApi.Services.Mappers
{
    public class TerritoryCardMapper : IDynamoDbEntityMapper<TerritoryCard>
    {
        public static class Keys
        {
            public static readonly string CardId = "card_id";
            internal static readonly string Neighborhood = "neighborhood";
            internal static readonly string Directions  = "directions";
            internal static readonly string IsDeleted = "is_deleted";
            internal static readonly string ShareId = "share_id";
            public static readonly string DirectionsCount = "directions_count";
        }

        public Document ToDynamoDocument(TerritoryCard card)    
        {
            var document = new Document();
            document[Keys.CardId] = card.CardId;
            document[Keys.Neighborhood] = card.Neighborhood;
            document[Keys.Directions] = JsonConvert.SerializeObject(card.Directions);
            document[Keys.DirectionsCount] = card.Directions.Count;
            return document;
        }

        public TerritoryCard ToEntity(Document result) => new TerritoryCard
        {
            CardId = MapCardId(result),
            Neighborhood = result[Keys.Neighborhood],
            Directions = MapDirections(result)
        };

        public static int MapCardId(Document document) => Convert.ToInt32(document[Keys.CardId]);
        public static List<Direction> MapDirections(Document document) => JsonConvert.DeserializeObject<List<Direction>>(document[Keys.Directions].AsString()) ?? new List<Direction>();

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

        public int ToCount(Document doc)
        {
            return doc.ContainsKey(Keys.DirectionsCount) ? Convert.ToInt32(doc[Keys.DirectionsCount]) : 0;
        }
    }
}