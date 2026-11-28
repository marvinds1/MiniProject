using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories.ToDoDetail
{
    public class TodoDetailRepository : ITodoDetailRepository
    {
        private readonly ApplicationDBContext _context;

        public TodoDetailRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        private IQueryable<TodoDetails1> OwnedBy(string userId) =>
            _context.TodoDetails.Where(d => _context.Todos.Any(t => t.TodoId == d.TodoId && t.UserId == userId));

        private static TodoDetailResponse Map(TodoDetails1 d) => new()
        {
            TodoDetailId = d.TodoDetailId,
            TodoId = d.TodoId,
            Activity = d.Activity,
            Category = d.Category,
            DetailNote = d.DetailNote
        };

        public Task<TodoDetailResponse?> GetById(Guid id, string userId, CancellationToken ct = default)
        {
            return OwnedBy(userId).AsNoTracking()
                .Where(d => d.TodoDetailId == id)
                .Select(d => new TodoDetailResponse
                {
                    TodoDetailId = d.TodoDetailId,
                    TodoId = d.TodoId,
                    Activity = d.Activity,
                    Category = d.Category,
                    DetailNote = d.DetailNote
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<TodoDetailResponse>?> GetByTodo(Guid todoId, string userId, CancellationToken ct = default)
        {
            if (!await _context.Todos.AnyAsync(t => t.TodoId == todoId && t.UserId == userId, ct))
                return null;

            return await _context.TodoDetails.AsNoTracking()
                .Where(d => d.TodoId == todoId)
                .OrderBy(d => d.Activity).ThenBy(d => d.TodoDetailId)
                .Select(d => new TodoDetailResponse
                {
                    TodoDetailId = d.TodoDetailId,
                    TodoId = d.TodoId,
                    Activity = d.Activity,
                    Category = d.Category,
                    DetailNote = d.DetailNote
                })
                .ToListAsync(ct);
        }

        public async Task<PagedResult<TodoWithDetails>> GetPage(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Todos.AsNoTracking().Where(t => t.UserId == userId);
            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(t => t.todayDate).ThenBy(t => t.TodoId)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(t => new TodoWithDetails
                {
                    TodoId = t.TodoId,
                    Day = t.day,
                    TodayDate = t.todayDate,
                    Note = t.note,
                    DetailCount = t.TodoDetails!.Count(),
                    Details = t.TodoDetails!
                        .OrderBy(d => d.Activity)
                        .Select(d => new TodoDetailResponse
                        {
                            TodoDetailId = d.TodoDetailId,
                            TodoId = d.TodoId,
                            Activity = d.Activity,
                            Category = d.Category,
                            DetailNote = d.DetailNote
                        })
                        .ToList()
                })
                .AsSplitQuery()
                .ToListAsync(ct);

            return new PagedResult<TodoWithDetails> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
        }

        public async Task<IReadOnlyList<TodoDetailResponse>?> CreateMany(Guid todoId, string userId, IReadOnlyList<NewTodoDetail> items, CancellationToken ct = default)
        {
            if (!await _context.Todos.AnyAsync(t => t.TodoId == todoId && t.UserId == userId, ct))
                return null;

            var entities = items.Select(i => new TodoDetails1
            {
                TodoDetailId = Guid.NewGuid(),
                TodoId = todoId,
                Activity = i.Activity,
                Category = i.Category,
                DetailNote = i.DetailNote
            }).ToList();

            _context.TodoDetails.AddRange(entities);
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                // parent todo was deleted concurrently (FK violation)
                return null;
            }

            return entities.Select(Map).ToList();
        }

        public async Task<bool> Update(Guid id, string userId, NewTodoDetail values, CancellationToken ct = default)
        {
            var rows = await OwnedBy(userId)
                .Where(d => d.TodoDetailId == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.Activity, values.Activity)
                    .SetProperty(d => d.Category, values.Category)
                    .SetProperty(d => d.DetailNote, values.DetailNote), ct);
            return rows == 1;
        }

        public async Task<string?> Delete(Guid id, string? requiredOwnerId, CancellationToken ct = default)
        {
            var owner = await _context.TodoDetails
                .Where(d => d.TodoDetailId == id)
                .Join(_context.Todos, d => d.TodoId, t => t.TodoId, (d, t) => new { t.UserId })
                .FirstOrDefaultAsync(ct);

            if (owner is null || (requiredOwnerId is not null && owner.UserId != requiredOwnerId))
                return null;

            var rows = await _context.TodoDetails.Where(d => d.TodoDetailId == id).ExecuteDeleteAsync(ct);
            return rows == 1 ? owner.UserId ?? string.Empty : null;
        }
    }
}
