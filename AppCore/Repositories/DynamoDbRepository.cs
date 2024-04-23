using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using CongEspVilaGuilhermeApi.Services.Mappers;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public abstract class DynamoDbRepository<Entity, Mapper> where Mapper : IDynamoDbEntityMapper<Entity>
    {
        public readonly static string domainName = "cong-esp-vlguilherme";

        public abstract string tableName { get; }
        public abstract Mapper mapper { get; }

        public AmazonDynamoDBClient Client
        {

            get
            {
                string accessKey = Settings.DynamoDBAccessKey;
                string secretKey = Settings.DynamoDBSecretKey;
                return new AmazonDynamoDBClient(accessKey, secretKey, Amazon.RegionEndpoint.USEast1);
            }
        }

        public Table Table
        {
            get
            {
                return Table.LoadTable(Client, tableName);
            }
        }

    }
}
