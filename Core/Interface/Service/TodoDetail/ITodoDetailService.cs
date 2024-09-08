using Core.Features.TodoDetail;
using Persistence.Models;

namespace Core.Interface.Service.TodoDetail
{
    public interface ITodoDetailService
    {
        Task<TodoDetails1> GetTodoDetailById(Guid id);
        Task<IEnumerable<TodoWithDetails>> GetAllTodoDetails();
        Task CreateTodoDetail(CreateTodoDetailQuery todoDetail);
        Task UpdateTodoDetail(UpdateTodoDetailQuery todoDetail);
        Task DeleteTodoDetail(Guid id);
    }
}
