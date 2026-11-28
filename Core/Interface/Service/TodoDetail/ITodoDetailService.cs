using Persistence.Models;

namespace Core.Interface.Service.TodoDetail
{
    public interface ITodoDetailService
    {
        Task<TodoDetailResponse?> Get(Guid id, string userId, CancellationToken ct = default);
        Task<IReadOnlyList<TodoDetailResponse>?> GetByTodo(Guid todoId, string userId, CancellationToken ct = default);
        Task<PagedResult<TodoWithDetails>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default);
        Task<IReadOnlyList<TodoDetailResponse>?> CreateMany(Guid todoId, string userId, IReadOnlyList<NewTodoDetail> items, CancellationToken ct = default);
        Task<TodoDetailResponse?> Update(Guid id, string userId, NewTodoDetail values, CancellationToken ct = default);
        Task<bool> Delete(Guid id, string userId, bool isAdmin, CancellationToken ct = default);
    }
}
