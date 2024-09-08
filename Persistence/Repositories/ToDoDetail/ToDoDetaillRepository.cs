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

        public async Task<TodoDetails1> GetTodoDetailById(Guid id)
        {
            return await _context.TodoDetails
                .Include(td => td.Todo)
                .FirstOrDefaultAsync(td => td.TodoDetailId == id);
        }

        public async Task<IEnumerable<TodoWithDetails>> GetAllTodoDetails()
        {
            return await _context.Todos
                .Select(todo => new TodoWithDetails
                {
                    Day = todo.day,
                    TodayDate = todo.todayDate,
                    Note = todo.note,
                    DetailCount = _context.TodoDetails.Count(detail => detail.TodoId == todo.TodoId),
                    Details = _context.TodoDetails
                        .Where(detail => detail.TodoId == todo.TodoId)
                        .Select(detail => new TodoDetailResponse
                        {
                            Activity = detail.Activity,
                            Category = detail.Category,
                            DetailNote = detail.DetailNote
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task CreateTodoDetail(TodoDetails1 todoDetail)
        {
            await _context.TodoDetails.AddAsync(todoDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTodoDetail(TodoDetails1 todoDetail)
        {
            _context.TodoDetails.Update(todoDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoDetail(Guid id)
        {
            var todoDetail = await _context.TodoDetails.FindAsync(id);
            if (todoDetail != null)
            {
                _context.TodoDetails.Remove(todoDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
