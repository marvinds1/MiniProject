using Core.Caching;
using Persistence.Models;
using Persistence.Repositories.ToDoDetail;

namespace Core.Interface.Service.TodoDetail
{
    public class TodoDetailService : ITodoDetailService
    {
        private readonly ITodoDetailRepository _details;
        private readonly UserDataCache _cache;

        public TodoDetailService(ITodoDetailRepository details, UserDataCache cache)
        {
            _details = details;
            _cache = cache;
        }

        public Task<TodoDetailResponse?> Get(Guid id, string userId, CancellationToken ct = default) =>
            _details.GetById(id, userId, ct);

        public Task<IReadOnlyList<TodoDetailResponse>?> GetByTodo(Guid todoId, string userId, CancellationToken ct = default) =>
            _details.GetByTodo(todoId, userId, ct);

        public Task<PagedResult<TodoWithDetails>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default) =>
            _cache.GetOrCreate(userId, $"details:p{page}:s{pageSize}", () => _details.GetPage(userId, page, pageSize, ct), ct);

        public async Task<IReadOnlyList<TodoDetailResponse>?> CreateMany(Guid todoId, string userId, IReadOnlyList<NewTodoDetail> items, CancellationToken ct = default)
        {
            var created = await _details.CreateMany(todoId, userId, items, ct);
            if (created is not null) await _cache.Invalidate(userId, ct);
            return created;
        }

        public async Task<TodoDetailResponse?> Update(Guid id, string userId, NewTodoDetail values, CancellationToken ct = default)
        {
            if (!await _details.Update(id, userId, values, ct)) return null;

            await _cache.Invalidate(userId, ct);
            return await _details.GetById(id, userId, ct);
        }

        public async Task<bool> Delete(Guid id, string userId, bool isAdmin, CancellationToken ct = default)
        {
            var owner = await _details.Delete(id, isAdmin ? null : userId, ct);
            if (owner is null) return false;

            await _cache.Invalidate(owner, ct);
            return true;
        }
    }
}
