using Mdb.Core;
using Mdb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Mdb.DAL;

public class UserRepository(IDbFactory dbFactory) : IUserRepository
{
    private readonly IMongoCollection<User> _users = dbFactory.GetCollection<User>("Users");

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _users.DeleteOneAsync(u => u.Id == userId);
    }

    public async IAsyncEnumerable<User>? GetAllAsync()
    {
        var cursor = await _users.FindAsync(new BsonDocument());
        while (await cursor.MoveNextAsync())
            foreach (var u in cursor.Current)
                yield return u;
    }

    public async Task<User?> GetByAsync(Guid id)
    {
        var user = await _users.FindAsync(u => u.Id == id);
        return await user.SingleOrDefaultAsync();
    }

    public async Task<List<User>> SearchAsync(string search)
    {
        return await _users.AsQueryable()
            .Where(u =>
                u.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                || u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync();
    }
}