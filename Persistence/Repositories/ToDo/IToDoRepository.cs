using System;
using Persistence.Models;

namespace Persistence.Repositories.ToDo
{
    public interface ITodoRepository
    {
        Task<Todo1?> GetTodoById(Guid id);
        Task<IEnumerable<Todo1>> GetAllTodos();
        Task<Todos> CreateTodo(string day, string note);
        Task<Todo1> UpdateTodo(Todos item);
        Task<bool> DeleteTodoById(Guid id);
    }
};


