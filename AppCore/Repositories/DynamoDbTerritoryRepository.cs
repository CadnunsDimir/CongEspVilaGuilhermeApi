using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CongEspVilaGuilhermeApi.AppCore.Models;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Services.Mappers;
using Newtonsoft.Json.Linq;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class DynamoDbTerritoryRepository : DynamoDbRepository<TerritoryCard, TerritoryCardMapper>, ITerritoryRepository
    {
        public override string tableName => $"{domainName}-territory-cards";

        public override TerritoryCardMapper mapper => new TerritoryCardMapper();
        private readonly string cardIdKey = TerritoryCardMapper.Keys.CardId;
        private readonly string shareIdKey = TerritoryCardMapper.Keys.ShareId;

        private Search queryBy(string key, DynamoDBEntry value, SelectValues select, List<string>? attributes = null)
        {
            CheckIfContainsValidAttributes(select, attributes);

            return Table.Scan(new ScanOperationConfig(){
                Filter = filterFrom(key, value),
                Select = select,
                AttributesToGet = attributes
            });
        }

        private static void CheckIfContainsValidAttributes(SelectValues select, List<string>? attributes = null)
        {
            if (select == SelectValues.SpecificAttributes && attributes == null)
                throw new ArgumentNullException(nameof(attributes));
        }

        private static ScanFilter filterFrom(string key, DynamoDBEntry value)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition(key, ScanOperator.Equal, value);
            return scanFilter;
        }

        private Search queryById(int id, SelectValues select, List<string>? attributes = null)
        {
            return queryBy(cardIdKey, id, select, attributes);
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

        public Task<List<int>> GetCardsAsync()=> GetCards();

        private async Task<List<int>> GetCards(ScanFilter? filter = null)
        {
            var config = new ScanOperationConfig()
            {
                AttributesToGet = new List<string> { cardIdKey, TerritoryCardMapper.Keys.IsDeleted },
                Select = SelectValues.SpecificAttributes
            };

            if(filter != null){
                config.Filter = filter;
            }

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
            var card = await GetCardAsync(id);
            CheckCardCanBeDeleted(card);
            await Table.DeleteItemAsync(mapper.ToDynamoDocument(card!));            
        }

        private static void CheckCardCanBeDeleted(TerritoryCard? card)
        {
            if (card == null)
                throw new InvalidOperationException("Entity does not exists");
            if (card.Directions.Count > 0)
                throw new InvalidOperationException("Territory contains directions. Move all to another card or delete the entries");
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

        public Task<List<TerritoryCard>> GetAll() => GetManyByFilter(null);

        public async Task<List<TerritoryCard>> GetManyByFilter(ScanFilter? filter = null)
        {
            var config = new ScanOperationConfig()
            {
                Select = SelectValues.AllAttributes
            };

            if (filter != null)
            {
                config.Filter = filter;
            }

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

        public async Task<List<TerritoryMapMarkers>> GetFullMapMarkers()
        {
            var query = Table.Scan(new ScanOperationConfig()
            {
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string>{ cardIdKey, TerritoryCardMapper.Keys.Directions}
            });

            var resultList = new List<TerritoryMapMarkers>();
            do
            {
                var documentList = await query.GetNextSetAsync();
                resultList.AddRange(mapper.ToMapMarkers(documentList));
            } while (!query.IsDone);

            return resultList;
        }

        public async Task<int> CountAllDirections()
        {
            await UpdateDirectionsSums();

            var countKey = TerritoryCardMapper.Keys.DirectionsCount;

            var scanFilter = new ScanFilter();
            scanFilter.AddCondition(countKey, ScanOperator.IsNotNull);
            scanFilter.AddCondition(TerritoryCardMapper.Keys.IsDeleted, ScanOperator.IsNull);
            
            var config = new ScanOperationConfig()
            {
                AttributesToGet = new List<string> { countKey },
                Filter = scanFilter,
                Select = SelectValues.SpecificAttributes
            };

            Search search = Table.Scan(config);
            var response = new List<int>();
            do
            {
                var documentList = await search.GetNextSetAsync();
                response.AddRange(documentList.Select(mapper.ToCount));
            } while (!search.IsDone);
            return response.Sum();
        }

        private static ScanFilter Where(string key, ScanOperator op, string? value = null)
        {
            ScanFilter filter = new ScanFilter();
            if(value != null) 
                filter.AddCondition(key, op, value);
            else filter.AddCondition(key,op);
            return filter;
        }

        private async Task UpdateDirectionsSums()
        {            
            var result  = await GetManyByFilter(Where(TerritoryCardMapper.Keys.DirectionsCount,  ScanOperator.IsNull));
            result.ForEach(async x => await Update(x));
        }        

        public Task<List<int>> GetCardsToFixCoordinates()
        {
            var invalidCoordinates = "\"Lat\":null";
            return GetCards(Where(TerritoryCardMapper.Keys.Directions, ScanOperator.Contains, invalidCoordinates));
        }        
    }
}
