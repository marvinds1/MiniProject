using MediatR;
using Persistence.Models;

namespace Application.Features.Todo.Commands
{
    public class CreateTodoCommand : IRequest<TodoResponse>
    {
        public string day { get; set; }
        public string note { get; set; }
    }
}
