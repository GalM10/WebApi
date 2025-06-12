using Mdb.Models;

namespace Mdb.Core;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid userId);
    public Task<User?> GetByAsync(Guid id);
    IAsyncEnumerable<User>? GetAllAsync();
}