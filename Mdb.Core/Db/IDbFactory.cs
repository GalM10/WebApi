
using MongoDB.Driver;

namespace Mdb.Core;

public interface IDbFactory
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
}