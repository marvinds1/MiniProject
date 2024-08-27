using MediatR;
using Persistence.Models;

namespace Application.Features.Todo.Commands
{
    public class CreateTodoCommand : IRequest<TodoResponse>
    {
        public TodoRequest Todo { get; set; }
    }
}
