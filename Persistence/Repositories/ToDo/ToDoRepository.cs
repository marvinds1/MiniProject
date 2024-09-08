using System;
using System.Linq;
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

        public async Task<IEnumerable<Todo1>> GetAllTodos()
        {
            return await _context.Todos.Select(todo => new Todo1
            {
                day = todo.day,
                todayDate = todo.todayDate,
                note = todo.note,
                detailCount = todo.detailCount
            }).ToListAsync();
        }

        public async Task<Todos> CreateTodo(string day, string note)
        {
            var creteTodo = new Todos
            {
                TodoId = Guid.NewGuid(),
                day = day,
                todayDate = DateTime.Now,
                note = note,
                detailCount = 0
            };

            try
            {
                _context.Todos.Add(creteTodo);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {}

            return creteTodo;
            
        }

        public async Task<Todo1?> GetTodoById(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            return todo == null ? null : MapToTodo1(todo);
        }

        public async Task<Todo1> UpdateTodo(Todos item)
        {
            var existingTodo = await _context.Todos.FindAsync(item.TodoId);

            if (existingTodo == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            // Update properties
            existingTodo.day = item.day;
            existingTodo.note = item.note;
            existingTodo.detailCount = item.detailCount;

            _context.Todos.Update(existingTodo);
            await _context.SaveChangesAsync();

            return MapToTodo1(existingTodo);
        }

        public async Task<bool> DeleteTodoById(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return false;
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return true;
        }

        private Todo1 MapToTodo1(Todos todo)
        {
            return new Todo1
            {
                todayDate = todo.todayDate,
                day = todo.day,
                note = todo.note,
                detailCount = todo.detailCount
            };
        }
    }

}
