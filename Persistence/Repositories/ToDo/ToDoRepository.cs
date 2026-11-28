using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories.ToDo
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDBContext _context;

        public TodoRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Todo1>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Todos.AsNoTracking().Where(t => t.UserId == userId);
            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(t => t.todayDate).ThenBy(t => t.TodoId)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(t => new Todo1
                {
                    todoId = t.TodoId,
                    day = t.day,
                    todayDate = t.todayDate,
                    note = t.note,
                    detailCount = t.TodoDetails!.Count(),
                    version = t.Version
                })
                .ToListAsync(ct);

            return new PagedResult<Todo1> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
        }

        public Task<Todo1?> GetById(Guid id, string userId, CancellationToken ct = default)
        {
            return _context.Todos.AsNoTracking()
                .Where(t => t.TodoId == id && t.UserId == userId)
                .Select(t => new Todo1
                {
                    todoId = t.TodoId,
                    day = t.day,
                    todayDate = t.todayDate,
                    note = t.note,
                    detailCount = t.TodoDetails!.Count(),
                    version = t.Version
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Todo1> Create(string userId, string day, string note, DateTime now, CancellationToken ct = default)
        {
            var todo = new Todos
            {
                TodoId = Guid.NewGuid(),
                UserId = userId,
                day = day,
                todayDate = now,
                note = note
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync(ct);

            return new Todo1 { todoId = todo.TodoId, day = day, todayDate = now, note = note, version = todo.Version };
        }

        public async Task<WriteResult> Update(Guid id, string userId, string day, string note, int expectedVersion, CancellationToken ct = default)
        {
            var rows = await _context.Todos
                .Where(t => t.TodoId == id && t.UserId == userId && t.Version == expectedVersion)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.day, day)
                    .SetProperty(t => t.note, note)
                    .SetProperty(t => t.Version, t => t.Version + 1), ct);

            if (rows == 1) return WriteResult.Ok;

            var exists = await _context.Todos.AnyAsync(t => t.TodoId == id && t.UserId == userId, ct);
            return exists ? WriteResult.Conflict : WriteResult.NotFound;
        }

        public async Task<string?> Delete(Guid id, string? requiredOwnerId, CancellationToken ct = default)
        {
            var query = _context.Todos.Where(t => t.TodoId == id);
            if (requiredOwnerId is not null)
                query = query.Where(t => t.UserId == requiredOwnerId);

            var owner = await query.Select(t => new { t.UserId }).FirstOrDefaultAsync(ct);
            if (owner is null) return null;

            var rows = await query.ExecuteDeleteAsync(ct);
            return rows == 1 ? owner.UserId ?? string.Empty : null;
        }

        public Task<int> AssignOwnerlessTo(string userId, CancellationToken ct = default)
        {
            return _context.Todos
                .Where(t => t.UserId == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.UserId, userId), ct);
        }
    }
}
