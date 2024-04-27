using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Services.Mappers;
using static CongEspVilaGuilhermeApi.Services.Mappers.UserMapper;

namespace CongEspVilaGuilhermeApi.AppCore.Repositories
{
    public class DynamoDbUserRepository : DynamoDbRepository<User, UserMapper>, IUserRepository
    {
        private readonly int internalId = 0;
        public override string tableName => $"{domainName}-users";
        public override UserMapper mapper => new UserMapper();

        public async Task<User?> GetByUserName(string userName)
        {

            var line = await QueryByUserName(userName);

            return line.Items
                .Select(x => Document.FromAttributeMap(x))
                .Select(x => mapper.ToEntity(x))
                .FirstOrDefault();
        }

        private Task<QueryResponse> QueryByUserName(string userName)
        {
            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = $"{Keys.internalId} = :k_id and username = :v_username",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_username", new AttributeValue { S =  userName }},
                    {":k_id", new AttributeValue { N =  "0" }}
                }
            };

            return Client.QueryAsync(request);
        }

        public Task Update(User user)
        {
            return Table.UpdateItemAsync(mapper.ToDynamoDocument(internalId, user));
        }

        public Task Create(User user)
        {
            return Table.PutItemAsync(mapper.ToDynamoDocument(internalId, user));
        }

        public async Task<bool> UserNameIsAvailable(string userName)
        {
            var line = await QueryByUserName(userName);

            return line.Count == 0;
        }
    }
}
