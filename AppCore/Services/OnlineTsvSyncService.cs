
using CongEspVilaGuilhermeApi.Domain.Entities;
using System.Net.Http.Headers;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class OnlineTsvSyncService : LoadFileService
    {
        /*baixar arquivo
         * coverter em objetos
         * comparar com os registros do DynamoDb
         * e atualizar os que não estiverem lá
         * sem deletar os dado de lat long
         */
        public OnlineTsvSyncService(IWebHostEnvironment hostingEnvironment) : base(hostingEnvironment)
        {
        }

        private List<TerritoryCard> getCardListFromTSVFileContent(string tsvFileContent)
        {
            var lines = tsvFileContent.Split('\n');
            var ignoreLines = new string[] {
                "DIRECCION", " ", "\t"
            };
            var cards = new List<TerritoryCard>();

            var fullDirectionKey = 0;
            var complementaryInfoKey = 1;
            var neighboorhoodKey = 2;
            var cardIdKey = 3;
            
            foreach ( var line in lines )
            {
                if(!ignoreLines.Any(x=> line.StartsWith(x)))
                {
                    var values = line.Split('\t');
                    var cardId = Convert.ToInt32(values[cardIdKey].ToLower().Replace("tarjeta ", string.Empty));
                    var neighboorhood = values[neighboorhoodKey];
                    var card = cards.FirstOrDefault(x => x.CardId == cardId) ?? new TerritoryCard
                    {
                        CardId = cardId,
                        Neighborhood = neighboorhood.Trim(),
                        Directions = new List<Direction>()
                    };

                    var adressData = values[fullDirectionKey];

                    var streetName = adressData;
                    var houseNumber = "S/N";

                    if (adressData.Contains(','))
                    {
                        var fullDirection = adressData.Split(',');
                        streetName = fullDirection.FirstOrDefault()!.Trim();
                        houseNumber = fullDirection.LastOrDefault()!.Trim();
                    }
                    else
                    {
                        var fullDirection = adressData.Split(' ');
                        houseNumber = fullDirection.LastOrDefault() ?? houseNumber;
                        streetName = streetName.Replace(houseNumber, string.Empty);                    
                    }
                    
                    card.Directions.Add(new Direction {
                        StreetName = streetName,
                        HouseNumber = houseNumber,
                        ComplementaryInfo = values[complementaryInfoKey]
                    });
                    if (!cards.Any(x=> x.CardId == cardId))
                    {
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }

        public async Task<List<TerritoryCard>> GetCardFromOnlineSheetAsync()
        {
            var tsvUrl = Settings.TsvUrl;
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            var tsv = await client.GetStringAsync(tsvUrl);
            return getCardListFromTSVFileContent(tsv);
        }
    }
}
