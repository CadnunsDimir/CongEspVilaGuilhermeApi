using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CongEspVilaGuilhermeApi.AppCore.Models;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Services.Mappers;
using Newtonsoft.Json.Linq;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class DynamoDbTerritoryRepository : DynamoDbRepository<TerritoryCard, TerritoryCardMapper>, ITerritoryRepository
    {
        public override string tableName => $"{domainName}-territory-cards";

        public override TerritoryCardMapper mapper => new TerritoryCardMapper();
        public string cardIdKey = TerritoryCardMapper.Keys.CardId;
        public string shareIdKey = TerritoryCardMapper.Keys.ShareId;

        private Search queryBy(string key, string value, SelectValues select, List<string>? attributes = null)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition(key, ScanOperator.Equal, new List<AttributeValue>
            {
                new AttributeValue(value)
            });

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

        private async Task<List<TerritoryCard>>  RunQueryAsync(Search query)
        {
            var resultList = new List<TerritoryCard>();
            do
            {
                var documentList = await query.GetNextSetAsync();
                resultList.AddRange(documentList.Select(mapper.ToEntity).ToList());
            } while (!query.IsDone);

            return resultList;
        }

        private async Task<TerritoryCard?> First(Search search)
        {
            var resultList = await RunQueryAsync(search);
            return resultList.FirstOrDefault();
        }

        public Task<TerritoryCard?> GetCardAsync(int id)
        {
            return First(queryById(id, SelectValues.AllAttributes));
        }

        public async Task<List<int>> GetCardsAsync()
        {
            var config = new ScanOperationConfig()
            {
                AttributesToGet = new List<string> { cardIdKey, TerritoryCardMapper.Keys.IsDeleted },
                Select = SelectValues.SpecificAttributes
            };

            Search search = Table.Scan(config);

            var resultList = new List<int>();

            do
            {
                var documentList = await search.GetNextSetAsync();
                resultList.AddRange(documentList
                    .Where(d=> 
                        !d.Contains(TerritoryCardMapper.Keys.IsDeleted) || 
                        !d[TerritoryCardMapper.Keys.IsDeleted].AsBoolean())
                    .Select(x => Convert.ToInt32(x[cardIdKey])).ToList());
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
                document[TerritoryCardMapper.Keys.IsDeleted] = true;
                await Table.UpdateItemAsync(document);
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

        public async Task<List<TerritoryCard>> GetAll()
        {
            var config = new ScanOperationConfig()
            {
                Select = SelectValues.AllAttributes
            };

            var list = await RunQueryAsync(Table.Scan(config));
            return list;
        }

        public async Task UpdateMany(List<TerritoryCard> territories)
        {
            var documents = territories.Select(x=> mapper.ToDynamoDocument(x)).ToList();
            foreach (var item in documents)
            {
                Console.WriteLine("[UpdateMany] update more 1...");
                await Table.UpdateItemAsync(item);
                await Task.Delay(500);
            }
            Console.WriteLine("[UpdateMany] update finished!");
        }

        public async Task UpdateShareableIdAsync(int cardId, Guid id)
        {
            var card = await GetCardAsync(cardId);
            var document = mapper.ToDynamoDocument(card!);
            document[shareIdKey]= id;
            await Table.UpdateItemAsync(document);
        }

        public Task<TerritoryCard?> GetByShareId(Guid cardId) => First(queryBy(shareIdKey,cardId.ToString(), SelectValues.AllAttributes));
    }
}
