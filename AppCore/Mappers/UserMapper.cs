using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Services.Mappers
{
    public class UserMapper : IDynamoDbEntityMapper<User>
    {
        public static class Keys
        {
            internal static readonly string Email = "email";
            internal static readonly string PasswordHash = "password_hash";
            internal static readonly string Role = "role";
            internal static readonly string UserName = "username";
            internal static readonly string internalId = "internal_Id";
        }

        public Document ToDynamoDocument(int internalId, User user)
        {
            var dynamoDocument = ToDynamoDocument(user);
            dynamoDocument[Keys.internalId] = internalId;
            return dynamoDocument;
        }

        public User ToEntity(Document result)
        {
            return new User
            {
                Email = result[Keys.Email],
                PasswordHash = result[Keys.PasswordHash],
                Role = Optional(result, Keys.Role),
                UserName = result[Keys.UserName]
            };
        }

        private string Optional(Document result, string attributeName) =>
            result.Contains(attributeName) ? result[attributeName] : string.Empty;

        public Document ToDynamoDocument(User user)
        {
            var dynamoDocument = new Document();
            dynamoDocument[Keys.Email] = user.Email;
            dynamoDocument[Keys.PasswordHash] = user.PasswordHash;
            dynamoDocument[Keys.Role] = user.Role;
            dynamoDocument[Keys.UserName] = user.UserName;
            return dynamoDocument;
        }
    }
}
