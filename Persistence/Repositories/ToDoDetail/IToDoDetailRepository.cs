using Persistence.Models;

namespace Persistence.Repositories.ToDoDetail;

public interface ITodoDetailRepository
{
    Task<TodoDetailResponse?> GetById(Guid id, string userId, CancellationToken ct = default);
    Task<IReadOnlyList<TodoDetailResponse>?> GetByTodo(Guid todoId, string userId, CancellationToken ct = default);
    Task<PagedResult<TodoWithDetails>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<TodoDetailResponse>?> CreateMany(Guid todoId, string userId, IReadOnlyList<NewTodoDetail> items, CancellationToken ct = default);
    Task<bool> Update(Guid id, string userId, NewTodoDetail values, CancellationToken ct = default);
    Task<string?> Delete(Guid id, string? requiredOwnerId, CancellationToken ct = default);
}
