using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Services.Mappers;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class DynamoDbTerritoryRepository : DynamoDbRepository<TerritoryCard, TerritoryCardMapper>, ITerritoryRepository
    {
        public override string tableName => $"{domainName}-territory-cards";

        public override TerritoryCardMapper mapper => new TerritoryCardMapper();
        public string cardIdKey = TerritoryCardMapper.Keys.CardId;

        private Search queryById(int id, SelectValues select, List<string>? attributes = null)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition(cardIdKey, ScanOperator.Equal, id);

            if (select == SelectValues.SpecificAttributes && attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            var config = new ScanOperationConfig()
            {
                Filter = scanFilter,
                Select = select,
                AttributesToGet = attributes
            };

            return Table.Scan(config);
        }

        public async Task<TerritoryCard?> GetCardAsync(int id)
        {
            var search = queryById(id, SelectValues.AllAttributes);
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

        public async Task Update(TerritoryCard card)
        {
            var entityExists = await verifyIfExists(card.CardId);
            if (entityExists)
            {
                await Table.UpdateItemAsync(mapper.ToDynamoDocument(card));
                return;
            }
            throw new InvalidOperationException();
        }

        private async Task<bool> verifyIfExists(int cardId)
        {
            var query = queryById(cardId, 
                SelectValues.SpecificAttributes,
                new List<string> { cardIdKey });

            var exists = false;
            while (!query.IsDone)
            {
                var result = await query.GetNextSetAsync();
                exists = result.Count > 0;
            }
            return exists;
        }

        public async Task Delete(int id)
        {
            var entityExists = await verifyIfExists(id);
            if (entityExists)
            {
                var document = await Table.GetItemAsync(id);
                await Table.DeleteItemAsync(document);
                return;
            }
            throw new InvalidOperationException();
        }

        public async Task UpdateDirection(int cardId, Direction direction)
        {
            var card = await GetCardAsync(cardId);
            var territory = card?.Directions.FirstOrDefault(x=>
                x.StreetName == direction.StreetName && x.HouseNumber == direction.HouseNumber);
            territory!.Lat = direction.Lat;
            territory.Long = direction.Long;
            await Update(card!);
        }
    }
}
