using System;
using MediatR;

namespace Application.Features.Todo.Commands
{
    public class DeleteTodoCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

