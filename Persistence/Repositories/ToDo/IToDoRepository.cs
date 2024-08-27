using System;
using Persistence.Models;

namespace Persistence.Repositories.ToDo
{
    public interface ITodoRepository
    {
        Task<Todo1> GetTodoByIdAsync(Guid id);
        Task<IEnumerable<Todo1>> GetAllTodosAsync();
        Task CreateTodoAsync(Todo1 todo);
        Task UpdateTodoAsync(Todo1 todo);
        Task DeleteTodoAsync(Guid id);
    }
};


