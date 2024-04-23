using CongEspVilaGuilhermeApi.AppCore.Repositories;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class TerritoryRepositoryValidationService
    {
        private TerritoryJsonRepository json;
        private DynamoDbTerritoryRepository dynamoDb;

        public TerritoryRepositoryValidationService(TerritoryJsonRepository json, DynamoDbTerritoryRepository dynamoDb)
        {
            this.json = json;
            this.dynamoDb = dynamoDb;
        }

        public async Task ValidateDataOnDynamoDb()
        {
            var countDb = await dynamoDb.GetCardsAsync();
            if (countDb?.Count == 0)
            {
                Console.WriteLine("DynamoDb Vazio. Preenchendo com json de territorios");
                var territoryJson = await json.GetCardsAsync();
                territoryJson.ForEach(async cardId =>
                {
                    var cardJson = await json.GetCardAsync(cardId);
                    if (cardJson != null)
                    {
                        await dynamoDb.Create(cardJson);
                    }
                });

                Console.WriteLine("DynamoDb carregado");
            }
        }
    }
}
