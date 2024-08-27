using MediatR;
using Persistence.Models;

namespace Application.Features.Todo.Commands
{
    public class UpdateTodoCommand : IRequest
    {
        public TodoRequest Todo { get; set; }
    }
}
