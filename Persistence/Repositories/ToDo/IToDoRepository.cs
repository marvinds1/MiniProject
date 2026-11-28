using Persistence.Models;

namespace Persistence.Repositories.ToDo
{
    public interface ITodoRepository
    {
        Task<Todo1?> GetById(Guid id, string userId, CancellationToken ct = default);
        Task<PagedResult<Todo1>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default);
        Task<Todo1> Create(string userId, string day, string note, DateTime now, CancellationToken ct = default);
        Task<WriteResult> Update(Guid id, string userId, string day, string note, int expectedVersion, CancellationToken ct = default);
        Task<string?> Delete(Guid id, string? requiredOwnerId, CancellationToken ct = default);
        Task<int> AssignOwnerlessTo(string userId, CancellationToken ct = default);
    }
}
