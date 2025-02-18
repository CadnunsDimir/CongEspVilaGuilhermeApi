using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Services.Mappers;
using Newtonsoft.Json;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class LifeAndMinistryWeekMapper : IDynamoDbEntityMapper<LifeAndMinistryWeek>
    {
        private const string EntitySerializedField = "EntitySerialized";
        private const string WeekLabelField = "WeekLabel";
        private const string IdField = "Id";
        public Document ToDynamoDocument(LifeAndMinistryWeek entity)
        {
            var document = new Document();
            document[WeekLabelField] = entity.WeekLabel;
            document[IdField] = entity.Id;
            document[EntitySerializedField] = JsonConvert.SerializeObject(entity);
            return document;
        }

        public LifeAndMinistryWeek ToEntity(Document result) => JsonConvert.DeserializeObject<LifeAndMinistryWeek>(result[EntitySerializedField]);
    }
}