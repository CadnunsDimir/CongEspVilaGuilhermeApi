using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using Newtonsoft.Json;
using ThirdParty.Json.LitJson;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class TerritoryJsonRepository : ITerritoryRepository
    {
        private readonly IWebHostEnvironment environment;
        private List<TerritoryCard>? cards = null;

        public TerritoryJsonRepository(IWebHostEnvironment hostingEnvironment)
        {
            environment = hostingEnvironment;
        }

        private List<TerritoryCard> LoadJson()
        {
            if (cards == null)
            {
                cards = new List<TerritoryCard>();
                var rootPath = environment.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "territorio.json");
                var jsonData = File.Exists(fullPath) ? File.ReadAllText(fullPath) : "[]";

                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    cards = ConvertToDynamicList(jsonData).Select(TerritoryCardSelector).ToList();
                }
            }

            return cards;
        }

        private TerritoryCard TerritoryCardSelector(dynamic data)
        {
            return new TerritoryCard
            {
                CardId = Convert.ToInt32(data.cartao.ToString()),
                Neighborhood = data.bairro,
                Directions = new List<dynamic>(data.enderecos).Select(x =>
                {
                    var complementaryInfo = new string[] { x.complemento1, x.complemento2 }
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Aggregate("", (current, next) => $"{current}, {next}");
                    var streetData = x.endereco.ToString().Split(",");
                    if (streetData.Length < 2)
                    {
                        string fullAddress = x.endereco.ToString().Trim();
                        var houseNumber = fullAddress.Split(" ").LastOrDefault() ?? "S/N";
                        var streetName = fullAddress.Replace(houseNumber, string.Empty);
                        streetData = new[] { streetName, houseNumber };
                    }
                    return new Direction
                    {
                        StreetName = streetData[0],
                        HouseNumber = streetData[1].Trim(),
                        ComplementaryInfo = complementaryInfo,
                    };
                }).ToList()
            };
        }

        private List<dynamic> ConvertToDynamicList(string jsonData)
        {
            return JsonConvert.DeserializeObject<List<dynamic>>(jsonData) ?? new List<dynamic>();
        }


        public Task<TerritoryCard?> GetCardAsync(int id)
        {
            return Task.FromResult(LoadJson().FirstOrDefault(x => x.CardId == id));
        }

        public Task<List<int>> GetCardsAsync()
        {
            return Task.FromResult(LoadJson().Select(x => x.CardId).ToList());
        }

        public Task Create(TerritoryCard card)
        {
            throw new NotImplementedException();
        }

        public Task Update(TerritoryCard card)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDirection(int cardId, Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
