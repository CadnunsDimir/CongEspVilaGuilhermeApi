using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class LifeAndMinistryDynamoDbRepository : DynamoDbRepository<LifeAndMinistryWeek, LifeAndMinistryWeekMapper>, ILifeAndMinistryRepository
    {
        public override string tableName => $"{domainName}-life-and-ministry-weeks";

        public override LifeAndMinistryWeekMapper mapper => new LifeAndMinistryWeekMapper();

        public Task CreateOrUpdate(LifeAndMinistryWeek week)
        {
            return Table.UpdateItemAsync(mapper.ToDynamoDocument(week));
        }

        public async Task<LifeAndMinistryWeek?> GetById(string weekId)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Id", ScanOperator.Equal, weekId);

            var config = new ScanOperationConfig()
            {
                Filter = scanFilter,
                Select = SelectValues.AllAttributes,
            };

            var search = Table.Scan(config);            

            LifeAndMinistryWeek? entity = null;
            do
            {
                var documentList = await search.GetNextSetAsync();
                entity = documentList.Select(mapper.ToEntity).FirstOrDefault();
            } while (!search.IsDone);

            return entity;
        }
    }
}
