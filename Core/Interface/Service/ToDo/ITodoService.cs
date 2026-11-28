using Persistence.Models;

namespace Core.Interface.Service.Todo
{
    public interface ITodoService
    {
        Task<Todo1?> Get(Guid id, string userId, CancellationToken ct = default);
        Task<PagedResult<Todo1>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default);
        Task<Todo1> Create(string userId, string day, string note, CancellationToken ct = default);
        Task<(WriteResult Result, Todo1? Todo)> Update(Guid id, string userId, string day, string note, int version, CancellationToken ct = default);
        Task<bool> Delete(Guid id, string userId, bool isAdmin, CancellationToken ct = default);
    }
}
