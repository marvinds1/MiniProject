//using System;
//using Microsoft.EntityFrameworkCore;
//using Persistence.DatabaseContext;
//using Persistence.Models;

//namespace Persistence.Repositories.ToDo
//{
//    public class TodoRepository : ITodoRepository
//    {
//        private readonly ApplicationDBContext _context;

//        public TodoRepository(ApplicationDBContext context)
//        {
//            _context = context;
//        }

//        public async Task<Todo1> GetTodoByIdAsync(Guid id)
//        {
//            return await _context.Todos
//                .Include(t => t.TodoDetails)
//                .FirstOrDefaultAsync(t => t.TodoId == id);
//        }

//        public async Task<IEnumerable<Todo1>> GetAllTodosAsync()
//        {
//            return await _context.Todos
//                .Include(t => t.TodoDetails)
//                .ToListAsync();
//        }

//        public async Task CreateTodoAsync(Todo1 todo)
//        {
//            _context.Todos.Add(todo);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateTodoAsync(Todo1 todo)
//        {
//            _context.Todos.Update(todo);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteTodoAsync(Guid id)
//        {
//            var todo = await _context.Todos.FindAsync(id);
//            if (todo != null)
//            {
//                _context.Todos.Remove(todo);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }

//}
