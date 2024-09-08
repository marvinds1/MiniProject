using System;
using Persistence.Models;

namespace Persistence.Repositories.ToDoDetail;

public interface ITodoDetailRepository
{
    Task<TodoDetails1> GetTodoDetailById(Guid id);
    Task<IEnumerable<TodoWithDetails>> GetAllTodoDetails();
    Task CreateTodoDetail(TodoDetails1 todoDetail);
    Task UpdateTodoDetail(TodoDetails1 todoDetail);
    Task DeleteTodoDetail(Guid id);
}
