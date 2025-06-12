using Mdb.Models;

namespace Mdb.Core;

public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<User>> SearchAsync(string search, CancellationToken cancellationToken = default);
    public Task<User?> GetByAsync(Guid id, CancellationToken cancellationToken = default);
    IAsyncEnumerable<User>? GetAllAsync(CancellationToken cancellationToken = default);
}