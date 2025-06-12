using System.Runtime.CompilerServices;
using Mdb.Core;
using Mdb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Mdb.DAL;

public class UserRepository(IDbFactory dbFactory) : IUserRepository
{
    private readonly IMongoCollection<User> _users = dbFactory.GetCollection<User>("Users");

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _users.DeleteOneAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }

    public async IAsyncEnumerable<User>? GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var cursor = await _users.FindAsync(new BsonDocument(), cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
            foreach (var u in cursor.Current)
                yield return u;
    }

    public async Task<User?> GetByAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);
        return await user.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<User>> SearchAsync(string search, CancellationToken cancellationToken = default)
    {
        return await _users.AsQueryable()
            .Where(u =>
                u.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                || u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync(cancellationToken);
    }
}