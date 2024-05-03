using CongEspVilaGuilhermeApi.AppCore.Repositories;
using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class TerritoryRepositoryValidationService
    {
        private readonly TerritoryJsonRepository json;
        private readonly DynamoDbTerritoryRepository dynamoDb;
        private readonly OnlineTsvSyncService onlineTsvSync;

        public TerritoryRepositoryValidationService(
            TerritoryJsonRepository json, 
            DynamoDbTerritoryRepository dynamoDb,
            OnlineTsvSyncService onlineTsvSync)
        {
            this.json = json;
            this.dynamoDb = dynamoDb;
            this.onlineTsvSync = onlineTsvSync;
        }

        public async Task ValidateDataOnDynamoDb()
        {
            var countDb = await dynamoDb.GetCardsAsync();
            if (countDb?.Count == 0)
            {
                Console.WriteLine("[ValidateDataOnDynamoDb] DynamoDb Vazio. Preenchendo com json de territorios");
                var territoryJson = await json.GetCardsAsync();
                territoryJson.ForEach(async cardId =>
                {
                    var cardJson = await json.GetCardAsync(cardId);
                    if (cardJson != null)
                    {
                        await dynamoDb.Create(cardJson);
                    }
                });

                Console.WriteLine("[ValidateDataOnDynamoDb] DynamoDb carregado");
            }
        }

        public async Task<string> UpdateDbUsingOnlineSheetAsync()
        {
            var cardsFromGoogleSheet = await onlineTsvSync.GetCardFromOnlineSheetAsync();
            Console.WriteLine(cardsFromGoogleSheet);
            var memoryDb = await dynamoDb.GetAll();
            var itensToUpdate = new List<TerritoryCard>();
            var changes = 0;
            var additions = 0;
            var deletions = 0;
            foreach (var tsvCard in cardsFromGoogleSheet)
            {
                var cardDb = memoryDb.FirstOrDefault(x=> x.CardId == tsvCard.CardId);
                if (cardDb == null)
                {
                    changes += tsvCard.Directions.Count;
                    additions += tsvCard.Directions.Count;
                    await dynamoDb.Create(tsvCard);
                }
                else
                {
                    tsvCard.Directions.ForEach(x =>
                    {
                        var directionDb = cardDb.Directions.FirstOrDefault(db =>
                            db.StreetName.Contains(x.StreetName) &&
                            db.HouseNumber.Contains(x.HouseNumber) &&
                            db.Lat != null);

                        if (directionDb != null)
                        {
                            x.Lat = directionDb.Lat;
                            x.Long = directionDb.Long;
                        }
                        else
                        {
                            changes++;
                            additions++;
                        }
                    });

                    var directionsBeingRemoved = cardDb.Directions.Count(x => !tsvCard.Directions.Any(db =>
                        db.StreetName.Contains(x.StreetName) &&
                        db.HouseNumber.Contains(x.HouseNumber)
                    ));

                    changes += directionsBeingRemoved;
                    deletions += directionsBeingRemoved;
               
                    cardDb.Directions = tsvCard.Directions;
                    itensToUpdate.Add(cardDb);
                }
            }
            await dynamoDb.UpdateMany(itensToUpdate);
            var successMessage = $"[TsvOnline] the database is up to date!";

            if (changes > 0)
            {
                successMessage += $"\n[TsvOnline] Synced OK | {changes} changes: {additions} additions and {deletions} deletions!";
            }
            
            Console.WriteLine(successMessage);
            return successMessage;
        }
    }
}
