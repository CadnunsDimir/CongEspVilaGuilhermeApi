using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Services.Mappers;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class DynamoDbTerritoryRepository : DynamoDbRepository<TerritoryCard, TerritoryCardMapper>, ITerritoryRepository
    {
        private TerritoryJsonRepository json;

        public override string tableName => $"{domainName}-territory-cards";

        public override TerritoryCardMapper mapper => new TerritoryCardMapper();
        public string cardIdKey = TerritoryCardMapper.Keys.CardId;

        public DynamoDbTerritoryRepository(TerritoryJsonRepository jsonRepository)
        {
            json = jsonRepository;
        }

        public async Task<TerritoryCard?> GetCardAsync(int id)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition(cardIdKey, ScanOperator.Equal, id);

            var config = new ScanOperationConfig()
            {
                Filter = scanFilter,
                Select = SelectValues.AllAttributes
            };

            Search search = Table.Scan(config);

            var resultList = new List<TerritoryCard>();

            do
            {
                var documentList = await search.GetNextSetAsync();
                resultList.AddRange(documentList.Select(mapper.ToEntity).ToList());
            } while (!search.IsDone);

            return resultList.FirstOrDefault();
        }

        public async Task<List<int>> GetCardsAsync()
        {
            // teste e2e aws
            var scanFilter = new ScanFilter();

            var config = new ScanOperationConfig()
            {
                AttributesToGet = new List<string> { cardIdKey },
                Select = SelectValues.SpecificAttributes
            };

            Search search = Table.Scan(config);

            var resultList = new List<int>();

            do
            {
                var documentList = await search.GetNextSetAsync();
                resultList.AddRange(documentList.Select(x => Convert.ToInt32(x[cardIdKey])).ToList());
            } while (!search.IsDone);

            return resultList.Order().ToList();
        }

        public Task Create(TerritoryCard card)
        {
            return Table.PutItemAsync(mapper.ToDynamoDocument(card));
        }
    }
}
