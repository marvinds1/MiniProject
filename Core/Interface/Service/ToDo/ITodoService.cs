using Core.Features.Todo;
using Persistence.Models;

namespace Core.Interface.Service.Todo
{
    public interface ITodoService
    {
        Task<Todo1> Get(Guid id);
        Task<IEnumerable<Todo1>> GetAll(bool cek = true);
        Task<Todo1> Create(CreateTodoQuery item);
        Task<Todo1> Update(Todos item);
        Task<bool> Delete(Guid id);
    }
}
