using Core.Caching;
using Persistence.Models;
using Persistence.Repositories.ToDo;

namespace Core.Interface.Service.Todo
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todos;
        private readonly UserDataCache _cache;
        private readonly TimeProvider _time;

        public TodoService(ITodoRepository todos, UserDataCache cache, TimeProvider time)
        {
            _todos = todos;
            _cache = cache;
            _time = time;
        }

        public Task<Todo1?> Get(Guid id, string userId, CancellationToken ct = default) =>
            _todos.GetById(id, userId, ct);

        public Task<PagedResult<Todo1>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default) =>
            _cache.GetOrCreate(userId, $"todos:p{page}:s{pageSize}", () => _todos.GetPage(userId, page, pageSize, ct), ct);

        public async Task<Todo1> Create(string userId, string day, string note, CancellationToken ct = default)
        {
            var todo = await _todos.Create(userId, day, note, _time.GetUtcNow().UtcDateTime, ct);
            await _cache.Invalidate(userId, ct);
            return todo;
        }

        public async Task<(WriteResult Result, Todo1? Todo)> Update(Guid id, string userId, string day, string note, int version, CancellationToken ct = default)
        {
            var result = await _todos.Update(id, userId, day, note, version, ct);
            if (result != WriteResult.Ok) return (result, null);

            await _cache.Invalidate(userId, ct);
            return (result, await _todos.GetById(id, userId, ct));
        }

        public async Task<bool> Delete(Guid id, string userId, bool isAdmin, CancellationToken ct = default)
        {
            var owner = await _todos.Delete(id, isAdmin ? null : userId, ct);
            if (owner is null) return false;

            await _cache.Invalidate(owner, ct);
            return true;
        }
    }
}
