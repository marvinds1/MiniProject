//using System;
//using Microsoft.EntityFrameworkCore;
//using Persistence.DatabaseContext;
//using Persistence.Models;

//namespace Persistence.Repositories.ToDoDetail
//{
//    public class TodoDetailRepository : ITodoDetailRepository
//    {
//        private readonly ApplicationDBContext _context;

//        public TodoDetailRepository(ApplicationDBContext context)
//        {
//            _context = context;
//        }

//        public async Task<TodoDetail1> GetTodoDetailByIdAsync(Guid id)
//        {
//            return await _context.TodoDetails
//                .FirstOrDefaultAsync(td => td.TodoDetailId == id);
//        }

//        public async Task<IEnumerable<TodoDetail1>> GetAllTodoDetailsAsync()
//        {
//            return await _context.TodoDetails.ToListAsync();
//        }

//        public async Task CreateTodoDetailAsync(TodoDetail1 todoDetail)
//        {
//            _context.TodoDetails.Add(todoDetail);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateTodoDetailAsync(TodoDetail1 todoDetail)
//        {
//            _context.TodoDetails.Update(todoDetail);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteTodoDetailAsync(Guid id)
//        {
//            var todoDetail = await _context.TodoDetails.FindAsync(id);
//            if (todoDetail != null)
//            {
//                _context.TodoDetails.Remove(todoDetail);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }

//}
