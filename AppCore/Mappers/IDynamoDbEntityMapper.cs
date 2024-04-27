using Amazon.DynamoDBv2.DocumentModel;
using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Services.Mappers
{
    public interface IDynamoDbEntityMapper<T>
    {
        Document ToDynamoDocument(T entity);
        T ToEntity(Document result);
    }

}